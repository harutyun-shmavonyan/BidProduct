using BidProduct.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BidProduct.DAL.DB
{
    public class BidProductDbContext : DbContext
    {
        public DbSet<Product>? Products { get; set; }

        public BidProductDbContext(DbContextOptions<BidProductDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelsBuilder)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
