﻿using Microsoft.EntityFrameworkCore;
using ShoppingCartGrpcMicroserviceApp.Models;

namespace ShoppingCartGrpcMicroserviceApp.Data
{
    public class ShoppingCartContext : DbContext
    {
        protected ShoppingCartContext(DbContextOptions<ShoppingCartContext> options) : base(options)
        {
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
