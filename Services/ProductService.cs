using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;
using System.ComponentModel.DataAnnotations;

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
        public async Task<(bool success, string message, Product? product)> CreateProductAsync(Product product)
        {
            try
            {
                // Validate
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(product);
                
                if (!Validator.TryValidateObject(product, validationContext, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                    return (false, $"Validation failed: {errors}", null);
                }

                // Check if category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
                if (!categoryExists)
                {
                    return (false, "Category does not exist", null);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return (true, "Product created successfully", product);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating product: {ex.Message}", null);
            }
        }

        // READ (Single)
        public async Task<(bool success, string message, Product? product)> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return (false, "Product not found", null);
                }

                return (true, "Product found", product);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving product: {ex.Message}", null);
            }
        }

        // READ (List with filtering)
        public async Task<(bool success, string message, List<Product> products)> GetProductsAsync(
            string? searchTerm = null, 
            int? categoryId = null)
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

                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                var products = await query.OrderBy(p => p.Name).ToListAsync();
                return (true, $"Found {products.Count} product(s)", products);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving products: {ex.Message}", new List<Product>());
            }
        }

        // UPDATE
        public async Task<(bool success, string message)> UpdateProductAsync(Product product)
        {
            try
            {
                // Validation
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(product);
                
                if (!Validator.TryValidateObject(product, validationContext, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                    return (false, $"Validation failed: {errors}");
                }

                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    return (false, "Product not found");
                }

                // Check if category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
                if (!categoryExists)
                {
                    return (false, "Category does not exist");
                }

                existingProduct.Name = product.Name;
                existingProduct.UnitPrice = product.UnitPrice;
                existingProduct.CategoryId = product.CategoryId;

                await _context.SaveChangesAsync();
                return (true, "Product updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating product: {ex.Message}");
            }
        }

        // DELETE
        public async Task<(bool success, string message)> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return (false, "Product not found");
                }

                // Check if product is used in any order lines
                var hasOrderLines = await _context.OrderLines.AnyAsync(ol => ol.ProductId == id);
                if (hasOrderLines)
                {
                    return (false, "Cannot delete product - it is referenced in existing orders");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return (true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting product: {ex.Message}");
            }
        }
    }
}
