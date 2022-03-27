namespace BidProduct.SL.Models.CQRS.Responses
{
    public class CreateProductCommandResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
