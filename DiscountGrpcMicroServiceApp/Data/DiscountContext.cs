using DiscountGrpcMicroServiceApp.Model;
using Microsoft.EntityFrameworkCore;

namespace DiscountGrpcMicroServiceApp.Data
{
    public class DiscountContext : DbContext
    {
        public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
        {
        }
        public DbSet<Discount> Discounts { get; set; }
    }
}
