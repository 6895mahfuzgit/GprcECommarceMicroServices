using Microsoft.EntityFrameworkCore;
using ProductGrpcMicroService.Models;

namespace ProductGrpcMicroService.Data
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        { }
        public DbSet<Product> Products { get; set; }
    }
}
