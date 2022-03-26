using System.Collections.Immutable;

namespace BidProduct.SL.Abstract.Validation
{
    public class ValidationResult<T>
    {
        public T Obj { get; }
        public ImmutableList<PropertyValidationError> Errors { get; }
        public bool HasErrors => Errors.Any();

        public ValidationResult(T obj, ICollection<PropertyValidationError> errors)
        {
            Obj = obj;
            var builder = ImmutableList.CreateBuilder<PropertyValidationError>();
            builder.AddRange(errors);
            Errors = builder.ToImmutable();
        }
    }
}