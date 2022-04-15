namespace BidProduct.SL.Models.CQRS.Responses
{
    public record CreateProductCommandResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
    }
}
