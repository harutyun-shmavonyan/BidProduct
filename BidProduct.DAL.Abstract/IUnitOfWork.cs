namespace BidProduct.DAL.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        void DisposeTransaction();
        void Commit();
        Task SaveAsync();
        void RollBack();
    }
}
