using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;

namespace OnlineShopping.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppContext> options) : base(options)
        {
        }
        
        public DbSet<Customer> Customers {get; set; }
        public DbSet<Category> Categories {get; set; }
        public DbSet<Order> Orders {get; set; }
        public DbSet<Product> Products {get; set; }
        public DbSet<OrderLine> OrderLines {get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seeding
            modelBuilder.Entity<Customer>().HasData( 
                new Customer {Id = 1, Name = "Sal", Email = "sal123@su.com", Adress = "Hamngatan 6"},
                new Customer {Id = 2, Name = "Amjad", Email = "amjad123@su.com", Adress = "Kistagången 22"}
            );
            modelBuilder.Entity<Category>().HasData(
                new Category {Id = 1, Name = "Sports Equipment", Description = "High-quality athletic gear and training accessories"},
                new Category {Id = 2, Name = "Summer clothing", Description = "Light and comfortable clothing for warm weather"}
            );
            modelBuilder.Entity<Order>().HasData(
                new Order {Id = 11, CustomerId = 1, Date = DateTime.Now, TotalAmount = 64.30M},
                new Order {Id = 12, CustomerId = 2, Date = DateTime.Now, TotalAmount = 85.70M}
            );
            modelBuilder.Entity<Product>().HasData(
                new Product {Id = 101, CategoryId = 1, Name = "Basketball", UnitPrice = 16.99m},
                new Product {Id = 102, CategoryId = 2, Name = "T-Shirt", UnitPrice = 11.99m}
            );
            modelBuilder.Entity<OrderLine>().HasData(
                new OrderLine { Id = 1001, OrderId = 11, ProductId = 101, UnitPrice = 16.99m, Quantity = 2 },
                new OrderLine { Id = 1002, OrderId = 11, ProductId = 101, UnitPrice = 16.99m, Quantity = 1 },
                new OrderLine { Id = 1003, OrderId = 12, ProductId = 102, UnitPrice = 11.99m, Quantity = 4 }
            );
        }
    
    

    
}
}