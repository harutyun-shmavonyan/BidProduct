using MediatR;
using BidProduct.Common.Exceptions;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Abstract.Validation;

namespace BidProduct.SL.Proxies
{
    public class InternalRequestHandlerValidatorProxy<TRequest, TResponse> : IInternalRequestHandler<TRequest, TResponse>
        where TRequest : IInternalRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _handler;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public InternalRequestHandlerValidatorProxy(IRequestHandler<TRequest, TResponse> handler, IEnumerable<IValidator<TRequest>>? validators = null)
        {
            _handler = handler;
            _validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
        }

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct)
        {
            var results = new List<ValidationResult<TRequest>>(_validators.Count());
            foreach (var validator in _validators)
            {
                var result = await validator.ValidateAsync(request);
                results.Add(result);
            }

            var propertyErrorMessages = results.Where(r => r.HasErrors)
                .Select(he => he.Errors)
                .SelectMany(e => e)
                .Select(e => e.ErrorMessage)
                .ToList();


            if (propertyErrorMessages.Any())
            {
                var errorMessage = string.Join(Environment.NewLine, propertyErrorMessages);
                throw new BidProductException(errorMessage, ExceptionType.InvalidOperation);
            }

            return await _handler.Handle(request, ct);
        }
    }
}