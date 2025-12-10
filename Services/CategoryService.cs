using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<(bool success, string message, Category? category)> CreateCategoryAsync(Category category)
        {
            try
            {
                // Validate
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(category);
                
                if (!Validator.TryValidateObject(category, validationContext, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                    return (false, $"Validation failed: {errors}", null);
                }

                // Check for duplicate category name
                var exists = await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.ToLower());
                if (exists)
                {
                    return (false, "A category with this name already exists", null);
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return (true, "Category created successfully", category);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating category: {ex.Message}", null);
            }
        }

        // READ (Single)
        public async Task<(bool success, string message, Category? category)> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return (false, "Category not found", null);
                }

                return (true, "Category found", category);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving category: {ex.Message}", null);
            }
        }

        // READ (List with filtering)
        public async Task<(bool success, string message, List<Category> categories)> GetCategoriesAsync(string? searchTerm = null)
        {
            try
            {
                var query = _context.Categories
                    .Include(c => c.Products)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm));
                }

                var categories = await query.OrderBy(c => c.Name).ToListAsync();
                return (true, $"Found {categories.Count} category(ies)", categories);
            }
            catch (Exception ex)
            {
                return (false, $"Error retrieving categories: {ex.Message}", new List<Category>());
            }
        }

        // UPDATE
        public async Task<(bool success, string message)> UpdateCategoryAsync(Category category)
        {
            try
            {
                // Validate
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(category);
                
                if (!Validator.TryValidateObject(category, validationContext, validationResults, true))
                {
                    var errors = string.Join(", ", validationResults.Select(v => v.ErrorMessage));
                    return (false, $"Validation failed: {errors}");
                }

                var existingCategory = await _context.Categories.FindAsync(category.Id);
                if (existingCategory == null)
                {
                    return (false, "Category not found");
                }

                // Check for duplicate name (excluding current category)
                var duplicateExists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id);
                if (duplicateExists)
                {
                    return (false, "A category with this name already exists");
                }

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;

                await _context.SaveChangesAsync();
                return (true, "Category updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating category: {ex.Message}");
            }
        }

        // DELETE
        public async Task<(bool success, string message)> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return (false, "Category not found");
                }

                // Check if category has products
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    return (false, "Cannot delete category - it has associated products");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return (true, "Category deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting category: {ex.Message}");
            }
        }
    }
}
