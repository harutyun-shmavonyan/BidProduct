namespace BidProduct.API.ViewModels.Product
{
    public record ProductReadViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
    }
}
