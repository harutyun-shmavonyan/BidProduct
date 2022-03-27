using FluentValidation;
using BidProduct.Common.Exceptions;
using BidProduct.SL.Abstract.Validation;

namespace BidProduct.SL.Validators.Abstract
{
    public abstract class SimpleValidatorBase<TInternalRequest> : AbstractValidator<TInternalRequest>, SL.Abstract.Validation.IValidator<TInternalRequest>
    {
        public Task<ValidationResult<TInternalRequest>> ValidateAsync(TInternalRequest entity)
        {
            var result = Validate(entity);
            return Task.FromResult(new ValidationResult<TInternalRequest>(entity, result.Errors.Select(e =>
                new PropertyValidationError(e.PropertyName, e.ErrorMessage)).ToList()));
        }

        public async Task ValidateAndThrowAsync(TInternalRequest entity)
        {
            var validationResult = await ValidateAsync(entity);
            if (validationResult.HasErrors)
            {
                throw new BidProductException(string.Join(Environment.NewLine,
                    validationResult.Errors.Select(e => e.ErrorMessage)), ExceptionType.ValidationFailed);
            }
        }

        public async Task ValidateAndThrowAsync(ICollection<TInternalRequest> entities)
        {
            var invalidEntityResults = new List<ValidationResult<TInternalRequest>>();
            foreach (var entity in entities)
            {
                var result = await ValidateAsync(entity);
                if (result.HasErrors)
                {
                    invalidEntityResults.Add(result);
                }
            }

            if (invalidEntityResults.Any())
            {
                throw new BidProductException(string.Join(Environment.NewLine,
                    invalidEntityResults.SelectMany(r => r.Errors).Select(e => e.ErrorMessage)),
                    ExceptionType.ValidationFailed);
            }
        }
    }
}