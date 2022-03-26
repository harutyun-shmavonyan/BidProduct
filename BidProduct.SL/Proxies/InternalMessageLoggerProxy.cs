using System.Collections.Concurrent;
using System.Text;
using BidProduct.SL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using BidProduct.SL.Extensions;

namespace BidProduct.SL.Proxies
{
    public class InternalMessageLoggerProxy
    {
        protected static readonly ConcurrentDictionary<string, int> NestingLevels = new();

        protected static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, int>> Durations = new();
    }

    public class InternalMessageLoggerProxy<TRequest, TResponse> : InternalMessageLoggerProxy, IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly ILogger _logger;
        private readonly InternalMessageLoggingConfiguration _configuration;
        private readonly IScopeIdProvider _scopeIdProvider;

        public InternalMessageLoggerProxy(IRequestHandler<TRequest, TResponse> handler,
            ILogger logger,
            IOptions<InternalMessageLoggingConfiguration> options,
            IScopeIdProvider scopeIdProvider)
        {
            _handler = handler;
            _logger = logger;
            _configuration = options.Value;
            _scopeIdProvider = scopeIdProvider;
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
                Durations.TryAdd(_scopeIdProvider.ScopeGuid, new ConcurrentDictionary<int, int>());
            }
            Durations[_scopeIdProvider.ScopeGuid].TryAdd(request.GetHashCode(), 0);

            var tabsBuilder = new StringBuilder();

            for (var i = 0; i < NestingLevels[_scopeIdProvider.ScopeGuid]; ++i)
            {
                tabsBuilder.Append("        ");
            }

            var prefix = tabsBuilder.ToString();

            _logger.Log($"{prefix}Request type: {typeof(TRequest).GetFullName()}");
            if (_configuration.ScopeIdNeeded) _logger.Log($"{prefix}Scope Id: {_scopeIdProvider.ScopeGuid}");
            if (_configuration.RequestStartDateNeeded) _logger.Log($"{prefix}Request started: {DateTime.UtcNow}");

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented
            };

            if (_configuration.RequestBodyLoggingNeeded)
            {
                var requestBody = JsonConvert.SerializeObject(request, serializerSettings);
                requestBody = requestBody.Replace(Environment.NewLine, Environment.NewLine + prefix);
                _logger.Log($"{prefix}Request body: {requestBody}");
            }

            var requestStartDate = DateTime.UtcNow;
            var response = await _handler.Handle(request, ct);
            var duration = (int)(DateTime.UtcNow - requestStartDate).TotalMilliseconds;

            Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()] += duration;
            foreach (var key in Durations[_scopeIdProvider.ScopeGuid].Keys)
            {
                if (key != request.GetHashCode())
                {
                    Durations[_scopeIdProvider.ScopeGuid][key] -= Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()];
                }
            }

            if (_configuration.ResponseBodyLoggingNeeded)
            {
                var responseBody = JsonConvert.SerializeObject(response, serializerSettings);
                responseBody = responseBody.Replace(Environment.NewLine, Environment.NewLine + prefix);
                _logger.Log($"{prefix}Response body: {responseBody}");
            }

            var clearDuration = Durations[_scopeIdProvider.ScopeGuid][request.GetHashCode()];
            if (_configuration.ClearDurationNeeded)
            {
                _logger.Log($"{prefix}Clear duration {clearDuration}ms");
                Durations[_scopeIdProvider.ScopeGuid].TryRemove(request.GetHashCode(), out _);
                if (!Durations[_scopeIdProvider.ScopeGuid].Any())
                {
                    Durations.TryRemove(_scopeIdProvider.ScopeGuid, out _);
                }
            }

            if (_configuration.DurationNeeded)
            {
                _logger.Log($"{prefix}Duration {duration}ms");
            }

            _configuration.MaxDurations.TryGetValue(request.GetType().Name, out var maxDuration);
            maxDuration = maxDuration > 0 ? maxDuration : _configuration.MaxDurations["Default"];
            if (clearDuration > maxDuration)
            {
                _logger.Log($"{prefix}Performance issue. Limit is {maxDuration}ms",
                    Microsoft.Extensions.Logging.LogLevel.Warning);
            }
            _logger.Log($"{prefix}------------------");

            --NestingLevels[_scopeIdProvider.ScopeGuid];
            if (NestingLevels[_scopeIdProvider.ScopeGuid] < 0)
            {
                NestingLevels.TryRemove(_scopeIdProvider.ScopeGuid, out _);
            }

            return response;
        }
    }
}