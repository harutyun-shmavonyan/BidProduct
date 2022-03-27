namespace BidProduct.SL.Abstract.Validation
{
    public interface IValidator<T>
    {
        public Task<ValidationResult<T>> ValidateAsync(T entity);
    }
}
