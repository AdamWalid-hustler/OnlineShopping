using OnlineShopping.Data;
using OnlineShopping.Models;
using OnlineShopping.Services;
using OnlineShopping.Utilities;

namespace OnlineShopping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Online Shopping Management System ===\n");

            using var context = new AppDbContext();
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n=== MAIN MENU ===");
                Console.WriteLine("1. Product Management");
                Console.WriteLine("2. Category Management");
                Console.WriteLine("3. Test Password Hashing");
                Console.WriteLine("4. Exit");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await ProductMenuAsync(productService, categoryService);
                        break;
                    case "2":
                        await CategoryMenuAsync(categoryService);
                        break;
                    case "3":
                        TestPasswordHashing();
                        break;
                    case "4":
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task ProductMenuAsync(ProductService productService, CategoryService categoryService)
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== PRODUCT MANAGEMENT ===");
                Console.WriteLine("1. List all products");
                Console.WriteLine("2. Search products");
                Console.WriteLine("3. View product details");
                Console.WriteLine("4. Create new product");
                Console.WriteLine("5. Update product");
                Console.WriteLine("6. Delete product");
                Console.WriteLine("7. Back to main menu");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await ListProductsAsync(productService);
                        break;
                    case "2":
                        await SearchProductsAsync(productService);
                        break;
                    case "3":
                        await ViewProductDetailsAsync(productService);
                        break;
                    case "4":
                        await CreateProductAsync(productService, categoryService);
                        break;
                    case "5":
                        await UpdateProductAsync(productService, categoryService);
                        break;
                    case "6":
                        await DeleteProductAsync(productService);
                        break;
                    case "7":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static async Task CategoryMenuAsync(CategoryService categoryService)
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== CATEGORY MANAGEMENT ===");
                Console.WriteLine("1. List all categories");
                Console.WriteLine("2. Search categories");
                Console.WriteLine("3. View category details");
                Console.WriteLine("4. Create new category");
                Console.WriteLine("5. Update category");
                Console.WriteLine("6. Delete category");
                Console.WriteLine("7. Back to main menu");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await ListCategoriesAsync(categoryService);
                        break;
                    case "2":
                        await SearchCategoriesAsync(categoryService);
                        break;
                    case "3":
                        await ViewCategoryDetailsAsync(categoryService);
                        break;
                    case "4":
                        await CreateCategoryAsync(categoryService);
                        break;
                    case "5":
                        await UpdateCategoryAsync(categoryService);
                        break;
                    case "6":
                        await DeleteCategoryAsync(categoryService);
                        break;
                    case "7":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // Product CRUD operations
        static async Task ListProductsAsync(ProductService productService)
        {
            var result = await productService.GetProductsAsync();
            if (result.success)
            {
                Console.WriteLine($"=== {result.message} ===");
                foreach (var product in result.products)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, " +
                                    $"Price: ${product.UnitPrice:F2}, Category: {product.Category?.Name ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {result.message}");
            }
        }

        static async Task SearchProductsAsync(ProductService productService)
        {
            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine();

            var result = await productService.GetProductsAsync(searchTerm);
            if (result.success)
            {
                Console.WriteLine($"\n=== {result.message} ===");
                foreach (var product in result.products)
                {
                    Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, " +
                                    $"Price: ${product.UnitPrice:F2}, Category: {product.Category?.Name ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {result.message}");
            }
        }

        static async Task ViewProductDetailsAsync(ProductService productService)
        {
            Console.Write("Enter product ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var result = await productService.GetProductByIdAsync(id);
                if (result.success && result.product != null)
                {
                    Console.WriteLine("\n=== Product Details ===");
                    Console.WriteLine($"ID: {result.product.Id}");
                    Console.WriteLine($"Name: {result.product.Name}");
                    Console.WriteLine($"Unit Price: ${result.product.UnitPrice:F2}");
                    Console.WriteLine($"Category: {result.product.Category?.Name ?? "N/A"}");
                }
                else
                {
                    Console.WriteLine($"Error: {result.message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static async Task CreateProductAsync(ProductService productService, CategoryService categoryService)
        {
            Console.WriteLine("=== Create New Product ===");
            
            // Show available categories
            var categoriesResult = await categoryService.GetCategoriesAsync();
            if (!categoriesResult.success || !categoriesResult.categories.Any())
            {
                Console.WriteLine("Error: No categories available. Please create a category first.");
                return;
            }

            Console.WriteLine("\nAvailable Categories:");
            foreach (var cat in categoriesResult.categories)
            {
                Console.WriteLine($"ID: {cat.Id}, Name: {cat.Name}");
            }

            Console.Write("\nEnter product name: ");
            var name = Console.ReadLine() ?? "";

            Console.Write("Enter unit price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            Console.Write("Enter category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid category ID format.");
                return;
            }

            var product = new Product
            {
                Name = name,
                UnitPrice = price,
                CategoryId = categoryId
            };

            var result = await productService.CreateProductAsync(product);
            Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
        }

        static async Task UpdateProductAsync(ProductService productService, CategoryService categoryService)
        {
            Console.Write("Enter product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingResult = await productService.GetProductByIdAsync(id);
            if (!existingResult.success || existingResult.product == null)
            {
                Console.WriteLine($"Error: {existingResult.message}");
                return;
            }

            Console.WriteLine($"\nCurrent Name: {existingResult.product.Name}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = existingResult.product.Name;

            Console.WriteLine($"Current Price: ${existingResult.product.UnitPrice:F2}");
            Console.Write("Enter new price (or press Enter to keep current): ");
            var priceInput = Console.ReadLine();
            decimal price = existingResult.product.UnitPrice;
            if (!string.IsNullOrWhiteSpace(priceInput) && !decimal.TryParse(priceInput, out price))
            {
                Console.WriteLine("Invalid price format.");
                return;
            }

            // Show available categories
            var categoriesResult = await categoryService.GetCategoriesAsync();
            Console.WriteLine("\nAvailable Categories:");
            foreach (var cat in categoriesResult.categories)
            {
                Console.WriteLine($"ID: {cat.Id}, Name: {cat.Name}");
            }

            Console.WriteLine($"Current Category ID: {existingResult.product.CategoryId}");
            Console.Write("Enter new category ID (or press Enter to keep current): ");
            var categoryInput = Console.ReadLine();
            int categoryId = existingResult.product.CategoryId;
            if (!string.IsNullOrWhiteSpace(categoryInput) && !int.TryParse(categoryInput, out categoryId))
            {
                Console.WriteLine("Invalid category ID format.");
                return;
            }

            var product = new Product
            {
                Id = id,
                Name = name,
                UnitPrice = price,
                CategoryId = categoryId
            };

            var result = await productService.UpdateProductAsync(product);
            Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
        }

        static async Task DeleteProductAsync(ProductService productService)
        {
            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write($"Are you sure you want to delete product ID {id}? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();
                if (confirm == "y" || confirm == "yes")
                {
                    var result = await productService.DeleteProductAsync(id);
                    Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
                }
                else
                {
                    Console.WriteLine("Delete cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Category CRUD operations
        static async Task ListCategoriesAsync(CategoryService categoryService)
        {
            var result = await categoryService.GetCategoriesAsync();
            if (result.success)
            {
                Console.WriteLine($"=== {result.message} ===");
                foreach (var category in result.categories)
                {
                    Console.WriteLine($"ID: {category.Id}, Name: {category.Name}, " +
                                    $"Description: {category.Description}, Products: {category.Products.Count}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {result.message}");
            }
        }

        static async Task SearchCategoriesAsync(CategoryService categoryService)
        {
            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine();

            var result = await categoryService.GetCategoriesAsync(searchTerm);
            if (result.success)
            {
                Console.WriteLine($"\n=== {result.message} ===");
                foreach (var category in result.categories)
                {
                    Console.WriteLine($"ID: {category.Id}, Name: {category.Name}, " +
                                    $"Description: {category.Description}, Products: {category.Products.Count}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {result.message}");
            }
        }

        static async Task ViewCategoryDetailsAsync(CategoryService categoryService)
        {
            Console.Write("Enter category ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var result = await categoryService.GetCategoryByIdAsync(id);
                if (result.success && result.category != null)
                {
                    Console.WriteLine("\n=== Category Details ===");
                    Console.WriteLine($"ID: {result.category.Id}");
                    Console.WriteLine($"Name: {result.category.Name}");
                    Console.WriteLine($"Description: {result.category.Description}");
                    Console.WriteLine($"Number of Products: {result.category.Products.Count}");
                    
                    if (result.category.Products.Any())
                    {
                        Console.WriteLine("\nProducts in this category:");
                        foreach (var product in result.category.Products)
                        {
                            Console.WriteLine($"  - {product.Name} (${product.UnitPrice:F2})");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {result.message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static async Task CreateCategoryAsync(CategoryService categoryService)
        {
            Console.WriteLine("=== Create New Category ===");
            
            Console.Write("Enter category name: ");
            var name = Console.ReadLine() ?? "";

            Console.Write("Enter description: ");
            var description = Console.ReadLine() ?? "";

            var category = new Category
            {
                Name = name,
                Description = description
            };

            var result = await categoryService.CreateCategoryAsync(category);
            Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
        }

        static async Task UpdateCategoryAsync(CategoryService categoryService)
        {
            Console.Write("Enter category ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingResult = await categoryService.GetCategoryByIdAsync(id);
            if (!existingResult.success || existingResult.category == null)
            {
                Console.WriteLine($"Error: {existingResult.message}");
                return;
            }

            Console.WriteLine($"\nCurrent Name: {existingResult.category.Name}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = existingResult.category.Name;

            Console.WriteLine($"Current Description: {existingResult.category.Description}");
            Console.Write("Enter new description (or press Enter to keep current): ");
            var description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                description = existingResult.category.Description;

            var category = new Category
            {
                Id = id,
                Name = name,
                Description = description
            };

            var result = await categoryService.UpdateCategoryAsync(category);
            Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
        }

        static async Task DeleteCategoryAsync(CategoryService categoryService)
        {
            Console.Write("Enter category ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write($"Are you sure you want to delete category ID {id}? (y/n): ");
                var confirm = Console.ReadLine()?.ToLower();
                if (confirm == "y" || confirm == "yes")
                {
                    var result = await categoryService.DeleteCategoryAsync(id);
                    Console.WriteLine(result.success ? $"Success: {result.message}" : $"Error: {result.message}");
                }
                else
                {
                    Console.WriteLine("Delete cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Password hashing test
        static void TestPasswordHashing()
        {
            Console.WriteLine("\n=== PASSWORD HASHING TEST ===\n");

            // Test 1: Hash a new password
            Console.Write("Enter a password to hash: ");
            var password = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty.");
                return;
            }

            Console.WriteLine("\n--- Hashing Password ---");
            var (hash, salt) = PasswordHasher.HashPassword(password);
            
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Salt (Base64): {salt}");
            Console.WriteLine($"Hash (Base64): {hash}");
            Console.WriteLine($"\nSalt Length: {Convert.FromBase64String(salt).Length} bytes");
            Console.WriteLine($"Hash Length: {Convert.FromBase64String(hash).Length} bytes");

            // Test 2: Verify correct password
            Console.WriteLine("\n--- Verification Test ---");
            Console.Write("Enter password again to verify: ");
            var testPassword = Console.ReadLine() ?? "";

            bool isValid = PasswordHasher.VerifyPassword(testPassword, hash, salt);
            Console.WriteLine($"Verification Result: {(isValid ? "✓ VALID" : "✗ INVALID")}");

            // Test 3: Verify wrong password
            Console.WriteLine("\n--- Wrong Password Test ---");
            Console.Write("Enter a different password: ");
            var wrongPassword = Console.ReadLine() ?? "";

            bool isWrong = PasswordHasher.VerifyPassword(wrongPassword, hash, salt);
            Console.WriteLine($"Verification Result: {(isWrong ? "✓ VALID" : "✗ INVALID (as expected)")}");

            // Test 4: Show that same password produces different hashes (different salts)
            Console.WriteLine("\n--- Unique Salt Test ---");
            Console.WriteLine("Hashing the same password twice produces different results:");
            var (hash1, salt1) = PasswordHasher.HashPassword(password);
            var (hash2, salt2) = PasswordHasher.HashPassword(password);
            
            Console.WriteLine($"\nFirst Hash:  {hash1.Substring(0, 20)}...");
            Console.WriteLine($"Second Hash: {hash2.Substring(0, 20)}...");
            Console.WriteLine($"Hashes are different: {hash1 != hash2}");
            Console.WriteLine($"But both verify correctly: {PasswordHasher.VerifyPassword(password, hash1, salt1) && PasswordHasher.VerifyPassword(password, hash2, salt2)}");

            Console.WriteLine("\n--- Security Information ---");
            Console.WriteLine("Algorithm: PBKDF2 (Password-Based Key Derivation Function 2)");
            Console.WriteLine("Hash Function: SHA256");
            Console.WriteLine("Iterations: 100,000 (key stretching for brute-force resistance)");
            Console.WriteLine("Salt Size: 16 bytes (128 bits) - unique per user");
            Console.WriteLine("Hash Size: 32 bytes (256 bits)");
            Console.WriteLine("\nThis prevents:");
            Console.WriteLine("- Rainbow table attacks (unique salts)");
            Console.WriteLine("- Brute force attacks (100k iterations make it slow)");
            Console.WriteLine("- Dictionary attacks (salt + iterations)");
        }
    }
}
