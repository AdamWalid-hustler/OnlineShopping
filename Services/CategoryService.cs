using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

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
        public Category? CreateCategory(string name, string description)
        {
            try
            {
                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Error: Category name is required");
                    return null;
                }

                if (name.Length < 2 || name.Length > 50)
                {
                    Console.WriteLine("Error: Category name must be between 2 and 50 characters");
                    return null;
                }

                if (!string.IsNullOrEmpty(description) && description.Length > 500)
                {
                    Console.WriteLine("Error: Description cannot exceed 500 characters");
                    return null;
                }

                // Check for duplicate category name
                var exists = _context.Categories.Any(c => c.Name.ToLower() == name.ToLower());
                if (exists)
                {
                    Console.WriteLine("Error: A category with this name already exists");
                    return null;
                }

                var category = new Category
                {
                    Name = name,
                    Description = description
                };

                _context.Categories.Add(category);
                _context.SaveChanges();
                
                Console.WriteLine("Category created successfully");
                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                return null;
            }
        }

        // READ (Single)
        public Category? GetCategoryById(int id)
        {
            try
            {
                var category = _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    Console.WriteLine("Error: Category not found");
                }

                return category;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving category: {ex.Message}");
                return null;
            }
        }

        // READ (List)
        public List<Category> GetCategories(string? searchTerm = null)
        {
            try
            {
                var query = _context.Categories
                    .Include(c => c.Products)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(c => c.Name.Contains(searchTerm));
                }

                var categories = query.OrderBy(c => c.Name).ToList();
                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
                return new List<Category>();
            }
        }

        // UPDATE
        public bool UpdateCategory(int id, string name, string description)
        {
            try
            {
                // Validate mandatory fields
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Error: Category name is required");
                    return false;
                }

                if (name.Length < 2 || name.Length > 50)
                {
                    Console.WriteLine("Error: Category name must be between 2 and 50 characters");
                    return false;
                }

                if (!string.IsNullOrEmpty(description) && description.Length > 500)
                {
                    Console.WriteLine("Error: Description cannot exceed 500 characters");
                    return false;
                }

                var category = _context.Categories.Find(id);
                if (category == null)
                {
                    Console.WriteLine("Error: Category not found");
                    return false;
                }

                // Check for duplicate name (excluding current category)
                var duplicateExists = _context.Categories
                    .Any(c => c.Name.ToLower() == name.ToLower() && c.Id != id);
                if (duplicateExists)
                {
                    Console.WriteLine("Error: A category with this name already exists");
                    return false;
                }

                category.Name = name;
                category.Description = description;

                _context.SaveChanges();
                Console.WriteLine("Category updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category: {ex.Message}");
                return false;
            }
        }

        // DELETE
        public bool DeleteCategory(int id)
        {
            try
            {
                var category = _context.Categories.Find(id);
                if (category == null)
                {
                    Console.WriteLine("Error: Category not found");
                    return false;
                }

                // Check if category has products
                var hasProducts = _context.Products.Any(p => p.CategoryId == id);
                if (hasProducts)
                {
                    Console.WriteLine("Error: Cannot delete category - it has associated products");
                    return false;
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
                Console.WriteLine("Category deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                return false;
            }
        }
    }
}
