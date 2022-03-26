namespace BidProduct.SL.Abstract.Validation
{
    public class PropertyValidationError
    {
        public string PropertyName { get; }
        public string ErrorMessage { get; }

        public PropertyValidationError(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
    }
}