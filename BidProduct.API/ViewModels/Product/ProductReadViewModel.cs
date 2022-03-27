namespace BidProduct.API.ViewModels.Product
{
    public class ProductReadViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
