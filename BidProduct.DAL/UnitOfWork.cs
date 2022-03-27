using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BidProduct.Common.Exceptions;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.DB;

namespace BidProduct.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BidProductDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(BidProductDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public void Commit()
        {
            if (_transaction == null)
                throw new InvalidOperationException("There is no transaction to commit");
            try
            {
                _transaction.Commit();
            }
            catch
            {
                RollBack();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public void RollBack()
        {
            _transaction?.Rollback();
            DisposeTransaction();
        }

        public async Task SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException is SqlException innerEx)
                    throw new BidProductException($"{string.Join(Environment.NewLine, innerEx.Errors)}",
                        ExceptionType.DbUpdateFailed);

                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                DisposeTransaction();
            }
        }

        public void DisposeTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}
