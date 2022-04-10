using System.Collections.Concurrent;
using System.Text;
using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BidProduct.SL.Extensions;
using BidProduct.Common.LogEvents;

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
        private readonly IScopeIdProvider _scopeIdProvider;
        private readonly IDateTimeService _dateTimeService;

        public InternalMessageLoggerProxy(
            IRequestHandler<TRequest, TResponse> handler,
            ILogger logger,
            IOptions<InternalMessageLoggingConfiguration> options,
            IScopeIdProvider scopeIdProvider,
            IDateTimeService dateTimeService)
        {
            _handler = handler;
            _logger = logger;
            _configuration = options.Value;
            _scopeIdProvider = scopeIdProvider;
            _dateTimeService = dateTimeService;
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            if (!NestingLevels.ContainsKey(_scopeIdProvider.ScopeGuid))
            {
                NestingLevels.TryAdd(_scopeIdProvider.ScopeGuid, -1);
            }
            ++NestingLevels[_scopeIdProvider.ScopeGuid];

            if (!Durations.ContainsKey(_scopeIdProvider.ScopeGuid))
            {
                Durations.TryAdd(_scopeIdProvider.ScopeGuid, new ConcurrentDictionary<int, TimeSpan>());
            }
            Durations[_scopeIdProvider.ScopeGuid].TryAdd(request.GetHashCode(), TimeSpan.Zero);

            var requestLogEvent = new RequestLogEvent<TRequest, TResponse>
            {
                Request = _configuration.RequestBodyLoggingNeeded ? request : default,
                NestingLevel = NestingLevels[_scopeIdProvider.ScopeGuid],
                Topics = { "InternalRequest" }
            };

            _logger.Log(requestLogEvent, Microsoft.Extensions.Logging.LogLevel.Information);

            //TODO ScopeId

            var requestStartDate = _dateTimeService.UtcNow;
            var response = await _handler.Handle(request, ct);
            var duration = (_dateTimeService.UtcNow - requestStartDate);

            Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()].Add(duration);
            foreach (var key in Durations[_scopeIdProvider.ScopeGuid].Keys)
            {
                if (key != request.GetHashCode())
                {
                    Durations[_scopeIdProvider.ScopeGuid][key] -= Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()];
                }
            }

            var clearDuration = Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()];
            if (_configuration.ClearDurationNeeded)
            {
                Durations[_scopeIdProvider.ScopeGuid].TryRemove(request.GetHashCode(), out _);
                if (!Durations[_scopeIdProvider.ScopeGuid].Any())
                {
                    Durations.TryRemove(_scopeIdProvider.ScopeGuid, out _);
                }
            }

            var responseLogEvent = new ResponseLogEvent<TResponse>
            {
                NestingLevel = NestingLevels[_scopeIdProvider.ScopeGuid],
                Topics = { "InternalResponse" },
                Response = _configuration.ResponseBodyLoggingNeeded ? response : default,
                ClearDuration = _configuration.ClearDurationNeeded ? clearDuration : default,
                Duration = _configuration.DurationNeeded ? duration : default
            };

            _logger.Log(responseLogEvent, Microsoft.Extensions.Logging.LogLevel.Information);

            _configuration.MaxDurations.TryGetValue(request.GetType().Name, out var maxDuration);
            maxDuration = maxDuration > 0 ? maxDuration : _configuration.MaxDurations["Default"];
            if (clearDuration > TimeSpan.FromMilliseconds(maxDuration))
            {
                _logger.Log(new PerformanceIssueLogEvent<TRequest, TResponse>
                {
                    Request = request,
                    Response = response,
                    Topics = { "InternalRequestPerformanceIssue" },
                }, Microsoft.Extensions.Logging.LogLevel.Warning);
            }

            --NestingLevels[_scopeIdProvider.ScopeGuid];
            if (NestingLevels[_scopeIdProvider.ScopeGuid] < 0)
            {
                NestingLevels.TryRemove(_scopeIdProvider.ScopeGuid, out _);
            }

            return response;
        }
    }
}