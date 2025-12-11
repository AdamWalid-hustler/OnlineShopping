using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public class ProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public Product? CreateProduct(string name, decimal price, int categoryId, int stock = 0)
        {
            try
            {
                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Error: Product name is required");
                    return null;
                }

                if (name.Length < 2 || name.Length > 100)
                {
                    Console.WriteLine("Error: Product name must be between 2 and 100 characters");
                    return null;
                }

                if (price <= 0)
                {
                    Console.WriteLine("Error: Unit price must be greater than 0");
                    return null;
                }

                if (stock < 0)
                {
                    Console.WriteLine("Error: Stock cannot be negative");
                    return null;
                }

                // Check if category exists
                var category = _context.Categories.Find(categoryId);
                if (category == null)
                {
                    Console.WriteLine("Error: Category does not exist");
                    return null;
                }

                var product = new Product
                {
                    Name = name,
                    UnitPrice = price,
                    CategoryId = categoryId,
                    Stock = stock
                };

                _context.Products.Add(product);
                _context.SaveChanges();
                
                Console.WriteLine("Product created successfully");
                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return null;
            }
        }

        // READ (Single)
        public Product? GetProductById(int id)
        {
            try
            {
                var product = _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.Id == id);

                if (product == null)
                {
                    Console.WriteLine("Error: Product not found");
                }

                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving product: {ex.Message}");
                return null;
            }
        }

        // READ (List with filtering)
        public List<Product> GetProducts(string? searchTerm = null)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(p => p.Name.Contains(searchTerm));
                }

                var products = query.OrderBy(p => p.Name).ToList();
                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving products: {ex.Message}");
                return new List<Product>();
            }
        }

        // UPDATE
        public bool UpdateProduct(int id, string name, decimal price, int categoryId, int stock)
        {
            try
            {
                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Error: Product name is required");
                    return false;
                }

                if (name.Length < 2 || name.Length > 100)
                {
                    Console.WriteLine("Error: Product name must be between 2 and 100 characters");
                    return false;
                }

                if (price <= 0)
                {
                    Console.WriteLine("Error: Unit price must be greater than 0");
                    return false;
                }

                if (stock < 0)
                {
                    Console.WriteLine("Error: Stock cannot be negative");
                    return false;
                }

                var product = _context.Products.Find(id);
                if (product == null)
                {
                    Console.WriteLine("Error: Product not found");
                    return false;
                }

                // Check if category exists
                var category = _context.Categories.Find(categoryId);
                if (category == null)
                {
                    Console.WriteLine("Error: Category does not exist");
                    return false;
                }

                product.Name = name;
                product.UnitPrice = price;
                product.CategoryId = categoryId;
                product.Stock = stock;

                _context.SaveChanges();
                Console.WriteLine("Product updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }

        // DELETE
        public bool DeleteProduct(int id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    Console.WriteLine("Error: Product not found");
                    return false;
                }

                // Check if product is used in any order lines
                var hasOrderLines = _context.OrderLines.Any(ol => ol.ProductId == id);
                if (hasOrderLines)
                {
                    Console.WriteLine("Error: Cannot delete product - it is used in existing orders");
                    return false;
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
                Console.WriteLine("Product deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }
    }
}
