using BidProduct.SL.Abstract.Validation;

namespace BidProduct.SL.Validators.Abstract
{
    public abstract class ValidatorBase<T>
    {
        private readonly ICollection<PropertyValidationError> _propertyErrors = new List<PropertyValidationError>();

        protected void RegisterPropertyError(string propertyName, string errorMessage) =>
            _propertyErrors.Add(new PropertyValidationError(propertyName, errorMessage));

        protected ValidationResult<T> Finish(T obj) => new(obj, _propertyErrors);
    }
}