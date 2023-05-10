using System.Collections.Concurrent;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.LogEvents;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ILogger = BidProduct.SL.Abstract.ILogger;
using BidProduct.Common.Abstract;

namespace BidProduct.SL.Proxies
{
    public class InternalMessageLoggerProxy
    {
        protected static readonly ConcurrentDictionary<string, int> NestingLevels = new();

        protected static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, TimeSpan>> Durations = new();
    }

    public class InternalMessageLoggerProxy<TRequest, TResponse> : InternalMessageLoggerProxy, IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly ILogger _logger;
        private readonly InternalMessageLoggingConfiguration _configuration;
        private readonly ITraceIdProvider _TraceIdProvider;
        private readonly IDateTimeService _dateTimeService;

        public InternalMessageLoggerProxy(
            IRequestHandler<TRequest, TResponse> handler,
            ILogger logger,
            IOptions<InternalMessageLoggingConfiguration> options,
            ITraceIdProvider TraceIdProvider,
            IDateTimeService dateTimeService)
        {
            _handler = handler;
            _logger = logger;
            _configuration = options.Value;
            _TraceIdProvider = TraceIdProvider;
            _dateTimeService = dateTimeService;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            if (!NestingLevels.ContainsKey(_TraceIdProvider.ScopeGuid))
            {
                NestingLevels.TryAdd(_TraceIdProvider.ScopeGuid, -1);
            }
            ++NestingLevels[_TraceIdProvider.ScopeGuid];

            if (!Durations.ContainsKey(_TraceIdProvider.ScopeGuid))
            {
                Durations.TryAdd(_TraceIdProvider.ScopeGuid, new ConcurrentDictionary<int, TimeSpan>());
            }
            Durations[_TraceIdProvider.ScopeGuid].TryAdd(request.GetHashCode(), TimeSpan.Zero);

            var requestLogEvent = new RequestLogEvent<TRequest, TResponse>
            {
                Request = _configuration.RequestBodyLoggingNeeded ? request : default,
                NestingLevel = NestingLevels[_TraceIdProvider.ScopeGuid]
            };

            _logger.Log(requestLogEvent, LogLevel.Information);

            //TODO TraceId

            var requestStartDate = _dateTimeService.UtcNow;

            TResponse response;
            try
            {
                response = await _handler.Handle(request, ct);
            }
            catch (Exception exception)
            {
                _logger.Log(new RequestFailedLogEvent<TRequest, TResponse>(request, exception)
                {
                    NestingLevel = NestingLevels[_TraceIdProvider.ScopeGuid]
                }, LogLevel.Error);

                throw;
            }

            var duration = (_dateTimeService.UtcNow - requestStartDate);

            Durations[_TraceIdProvider.ScopeGuid][request.GetHashCode()] = Durations[_TraceIdProvider.ScopeGuid][request.GetHashCode()].Add(duration);
            foreach (var key in Durations[_TraceIdProvider.ScopeGuid].Keys)
            {
                if (key != request.GetHashCode())
                {
                    Durations[_TraceIdProvider.ScopeGuid][key] -= Durations[_TraceIdProvider.ScopeGuid][request.GetHashCode()];
                }
            }

            var clearDuration = Durations[_TraceIdProvider.ScopeGuid][request.GetHashCode()];
            if (_configuration.ClearDurationNeeded)
            {
                Durations[_TraceIdProvider.ScopeGuid].TryRemove(request.GetHashCode(), out _);
                if (!Durations[_TraceIdProvider.ScopeGuid].Any())
                {
                    Durations.TryRemove(_TraceIdProvider.ScopeGuid, out _);
                }
            }

            var responseLogEvent = new ResponseLogEvent<TResponse>
            {
                NestingLevel = NestingLevels[_TraceIdProvider.ScopeGuid],
                Response = _configuration.ResponseBodyLoggingNeeded ? response : default,
                ClearDuration = _configuration.ClearDurationNeeded ? clearDuration : default,
                Duration = _configuration.DurationNeeded ? duration : default
            };

            _logger.Log(responseLogEvent, LogLevel.Information);

            _configuration.MaxDurations!.TryGetValue(request.GetType().Name, out var maxDuration);
            maxDuration = maxDuration > 0 ? maxDuration : _configuration.MaxDurations["Default"];
            if (clearDuration > TimeSpan.FromMilliseconds(maxDuration))
            {
                _logger.Log(new PerformanceIssueLogEvent<TRequest, TResponse>(request, response), LogLevel.Warning);
            }

            --NestingLevels[_TraceIdProvider.ScopeGuid];
            if (NestingLevels[_TraceIdProvider.ScopeGuid] < 0)
            {
                NestingLevels.TryRemove(_TraceIdProvider.ScopeGuid, out _);
            }

            return response;
        }
    }
}