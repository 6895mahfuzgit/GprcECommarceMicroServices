using DiscountGrpcMicroServiceApp.Model;
using Microsoft.EntityFrameworkCore;

namespace DiscountGrpcMicroServiceApp.Data
{
    public class DiscountContext : DbContext
    {
        protected DiscountContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Discount> Discounts { get; set; }
    }
}
