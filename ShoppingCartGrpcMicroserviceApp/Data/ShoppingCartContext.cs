using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpcMicroserviceApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartGrpcMicroserviceApp.Data
{
    public class ShoppingCartContext : DbContext
    {
        protected ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
        {
        }

        public DbSet<ShoppingCart>  ShoppingCarts{ get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
