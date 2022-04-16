using BidProduct.IdentityManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BidProduct.IdentityManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<BidProductUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
