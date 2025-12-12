using System;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Models;
using OnlineShopping.Helpers;

namespace OnlineShopping.Data
{
    public class AppDbContext : DbContext
    {
        // Parameterless constructor for design-time tools
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        
        // Database VIEW
        public DbSet<OrderSummaryView> OrderSummaryView { get; set; }
        
        // Price history table for trigger
        public DbSet<ProductPriceHistory> ProductPriceHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Provide a default Sqlite database for design-time tools/migrations when no options are supplied
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=onlineshopping.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DATABASE VIEW
            modelBuilder.Entity<OrderSummaryView>(entity =>
            {
                entity.HasNoKey(); // Views don't have primary keys
                entity.ToView("OrderSummaryView"); // Map to database view
            });

            // Configure ProductPriceHistory indexes
            modelBuilder.Entity<ProductPriceHistory>()
                .HasIndex(p => p.ProductId);
            
            modelBuilder.Entity<ProductPriceHistory>()
                .HasIndex(p => p.ChangedAt);

            // Configure indexes for performance
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerId);

            // Composite index for order queries with sorting
            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.Date, o.TotalAmount });

            // Index for date range queries
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Date);

            modelBuilder.Entity<OrderLine>()
                .HasIndex(ol => ol.OrderId);

            modelBuilder.Entity<OrderLine>()
                .HasIndex(ol => ol.ProductId);

            // Index for stock queries
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Stock);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Product)
                .WithMany(p => p.OrderLines)
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data with password hashing and address encryption
            var (hash1, salt1) = PasswordHasher.HashPassword("password123");
            var (hash2, salt2) = PasswordHasher.HashPassword("password456");
            
            // Encrypt addresses for security (application-level encryption)
            var encryptedAddress1 = EncryptionHelper.Encrypt("Hamngatan 6");
            var encryptedAddress2 = EncryptionHelper.Encrypt("Kistagången 22");

            modelBuilder.Entity<Customer>().HasData(
                new Customer 
                { 
                    Id = 1, 
                    Name = "Sal", 
                    Email = "sal123@su.com", 
                    Address = encryptedAddress1,  // Stored encrypted in database
                    PasswordHash = hash1,
                    PasswordSalt = salt1
                },
                new Customer 
                { 
                    Id = 2, 
                    Name = "Amjad", 
                    Email = "amjad123@su.com", 
                    Address = encryptedAddress2,  // Stored encrypted in database
                    PasswordHash = hash2,
                    PasswordSalt = salt2
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Sports Equipment", Description = "High-quality athletic gear and training accessories" },
                new Category { Id = 2, Name = "Summer clothing", Description = "Light and comfortable clothing for warm weather" }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 11, CustomerId = 1, Date = new DateTime(2024, 01, 01), TotalAmount = 64.30M },
                new Order { Id = 12, CustomerId = 2, Date = new DateTime(2024, 01, 02), TotalAmount = 85.70M }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 101, CategoryId = 1, Name = "Basketball", UnitPrice = 16.99m, Stock = 50 },
                new Product { Id = 102, CategoryId = 2, Name = "T-Shirt", UnitPrice = 11.99m, Stock = 100 }
            );

            modelBuilder.Entity<OrderLine>().HasData(
                new OrderLine { Id = 1001, OrderId = 11, ProductId = 101, UnitPrice = 16.99m, Quantity = 2 },
                new OrderLine { Id = 1002, OrderId = 11, ProductId = 101, UnitPrice = 16.99m, Quantity = 1 },
                new OrderLine { Id = 1003, OrderId = 12, ProductId = 102, UnitPrice = 11.99m, Quantity = 4 }
            );
        }
    }
}