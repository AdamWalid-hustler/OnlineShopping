using OnlineShopping.Data;
using OnlineShopping.Models;
using OnlineShopping.Services;
using Microsoft.EntityFrameworkCore;

namespace OnlineShopping
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Online Shopping System ===\n");

            using var context = new AppDbContext();
            var productService = new ProductService(context);
            var categoryService = new CategoryService(context);
            var orderService = new OrderService(context);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n=== MAIN MENU ===");
                Console.WriteLine("1. Product Management");
                Console.WriteLine("2. Category Management");
                Console.WriteLine("3. Order Management");
                Console.WriteLine("4. Test Database View");
                Console.WriteLine("5. Test Encryption");
                Console.WriteLine("6. Exit");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ProductMenu(productService, categoryService);
                        break;
                    case "2":
                        CategoryMenu(categoryService);
                        break;
                    case "3":
                        OrderMenu(orderService, productService, context);
                        break;
                    case "4":
                        TestDatabaseView(orderService);
                        break;
                    case "5":
                        TestEncryption(context);
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void ProductMenu(ProductService productService, CategoryService categoryService)
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
                        ListProducts(productService);
                        break;
                    case "2":
                        SearchProducts(productService);
                        break;
                    case "3":
                        ViewProductDetails(productService);
                        break;
                    case "4":
                        CreateProduct(productService, categoryService);
                        break;
                    case "5":
                        UpdateProduct(productService, categoryService);
                        break;
                    case "6":
                        DeleteProduct(productService);
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

        static void CategoryMenu(CategoryService categoryService)
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== CATEGORY MANAGEMENT ===");
                Console.WriteLine("1. List all categories");
                Console.WriteLine("2. View category details");
                Console.WriteLine("3. Create new category");
                Console.WriteLine("4. Update category");
                Console.WriteLine("5. Delete category");
                Console.WriteLine("6. Back to main menu");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ListCategories(categoryService);
                        break;
                    case "2":
                        ViewCategoryDetails(categoryService);
                        break;
                    case "3":
                        CreateCategory(categoryService);
                        break;
                    case "4":
                        UpdateCategory(categoryService);
                        break;
                    case "5":
                        DeleteCategory(categoryService);
                        break;
                    case "6":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void OrderMenu(OrderService orderService, ProductService productService, AppDbContext context)
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== ORDER MANAGEMENT ===");
                Console.WriteLine("1. List all orders");
                Console.WriteLine("2. View order details");
                Console.WriteLine("3. Create new order");
                Console.WriteLine("4. Update order items");
                Console.WriteLine("5. Delete order");
                Console.WriteLine("6. View low stock products");
                Console.WriteLine("7. Back to main menu");
                Console.Write("Select option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ListOrders(orderService);
                        break;
                    case "2":
                        ViewOrderDetails(orderService);
                        break;
                    case "3":
                        CreateOrder(orderService, context);
                        break;
                    case "4":
                        UpdateOrder(orderService, context);
                        break;
                    case "5":
                        DeleteOrder(orderService);
                        break;
                    case "6":
                        ViewLowStockProducts(orderService);
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

        // Product Operations
        static void ListProducts(ProductService productService)
        {
            var products = productService.GetProducts();
            Console.WriteLine($"=== All Products ({products.Count}) ===");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, " +
                                $"Price: ${product.UnitPrice:F2}, Stock: {product.Stock}, " +
                                $"Category: {product.Category?.Name ?? "N/A"}");
            }
        }

        static void SearchProducts(ProductService productService)
        {
            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine();

            var products = productService.GetProducts(searchTerm);
            Console.WriteLine($"\n=== Found {products.Count} products ===");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, " +
                                $"Price: ${product.UnitPrice:F2}, Stock: {product.Stock}");
            }
        }

        static void ViewProductDetails(ProductService productService)
        {
            Console.Write("Enter product ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var product = productService.GetProductById(id);
                if (product != null)
                {
                    Console.WriteLine("\n=== Product Details ===");
                    Console.WriteLine($"ID: {product.Id}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Unit Price: ${product.UnitPrice:F2}");
                    Console.WriteLine($"Stock: {product.Stock}");
                    Console.WriteLine($"Category: {product.Category?.Name ?? "N/A"}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateProduct(ProductService productService, CategoryService categoryService)
        {
            Console.WriteLine("=== Create New Product ===");
            
            var categories = categoryService.GetCategories();
            if (categories.Count == 0)
            {
                Console.WriteLine("No categories available. Please create a category first.");
                return;
            }

            Console.WriteLine("\nAvailable Categories:");
            foreach (var cat in categories)
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

            Console.Write("Enter stock quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Invalid stock format.");
                return;
            }

            Console.Write("Enter category ID: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid category ID format.");
                return;
            }

            productService.CreateProduct(name, price, categoryId, stock);
        }

        static void UpdateProduct(ProductService productService, CategoryService categoryService)
        {
            Console.Write("Enter product ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var product = productService.GetProductById(id);
            if (product == null)
            {
                return;
            }

            Console.WriteLine($"\nCurrent Name: {product.Name}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = product.Name;

            Console.WriteLine($"Current Price: ${product.UnitPrice:F2}");
            Console.Write("Enter new price (or press Enter to keep current): ");
            var priceInput = Console.ReadLine();
            decimal price = product.UnitPrice;
            if (!string.IsNullOrWhiteSpace(priceInput))
            {
                if (!decimal.TryParse(priceInput, out price))
                {
                    Console.WriteLine("Invalid price format.");
                    return;
                }
            }

            Console.WriteLine($"Current Stock: {product.Stock}");
            Console.Write("Enter new stock (or press Enter to keep current): ");
            var stockInput = Console.ReadLine();
            int stock = product.Stock;
            if (!string.IsNullOrWhiteSpace(stockInput))
            {
                if (!int.TryParse(stockInput, out stock))
                {
                    Console.WriteLine("Invalid stock format.");
                    return;
                }
            }

            var categories = categoryService.GetCategories();
            Console.WriteLine("\nAvailable Categories:");
            foreach (var cat in categories)
            {
                Console.WriteLine($"ID: {cat.Id}, Name: {cat.Name}");
            }

            Console.WriteLine($"Current Category ID: {product.CategoryId}");
            Console.Write("Enter new category ID (or press Enter to keep current): ");
            var categoryInput = Console.ReadLine();
            int categoryId = product.CategoryId;
            if (!string.IsNullOrWhiteSpace(categoryInput))
            {
                if (!int.TryParse(categoryInput, out categoryId))
                {
                    Console.WriteLine("Invalid category ID format.");
                    return;
                }
            }

            productService.UpdateProduct(id, name, price, categoryId, stock);
        }

        static void DeleteProduct(ProductService productService)
        {
            Console.Write("Enter product ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                productService.DeleteProduct(id);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Category Operations
        static void ListCategories(CategoryService categoryService)
        {
            var categories = categoryService.GetCategories();
            Console.WriteLine($"=== All Categories ({categories.Count}) ===");
            foreach (var category in categories)
            {
                Console.WriteLine($"ID: {category.Id}, Name: {category.Name}, " +
                                $"Products: {category.Products?.Count ?? 0}");
            }
        }

        static void ViewCategoryDetails(CategoryService categoryService)
        {
            Console.Write("Enter category ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var category = categoryService.GetCategoryById(id);
                if (category != null)
                {
                    Console.WriteLine("\n=== Category Details ===");
                    Console.WriteLine($"ID: {category.Id}");
                    Console.WriteLine($"Name: {category.Name}");
                    Console.WriteLine($"Description: {category.Description}");
                    Console.WriteLine($"Number of Products: {category.Products?.Count ?? 0}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateCategory(CategoryService categoryService)
        {
            Console.WriteLine("=== Create New Category ===");
            Console.Write("Enter category name: ");
            var name = Console.ReadLine() ?? "";

            Console.Write("Enter category description: ");
            var description = Console.ReadLine() ?? "";

            categoryService.CreateCategory(name, description);
        }

        static void UpdateCategory(CategoryService categoryService)
        {
            Console.Write("Enter category ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var category = categoryService.GetCategoryById(id);
            if (category == null)
            {
                return;
            }

            Console.WriteLine($"\nCurrent Name: {category.Name}");
            Console.Write("Enter new name (or press Enter to keep current): ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = category.Name;

            Console.WriteLine($"Current Description: {category.Description}");
            Console.Write("Enter new description (or press Enter to keep current): ");
            var description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
                description = category.Description;

            categoryService.UpdateCategory(id, name, description);
        }

        static void DeleteCategory(CategoryService categoryService)
        {
            Console.Write("Enter category ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                categoryService.DeleteCategory(id);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        // Order Operations
        static void ListOrders(OrderService orderService)
        {
            var orders = orderService.GetAllOrders();
            Console.WriteLine($"=== All Orders ({orders.Count}) ===");
            foreach (var order in orders)
            {
                Console.WriteLine($"ID: {order.Id}, Date: {order.Date:yyyy-MM-dd}, " +
                                $"Customer: {order.Customer?.Name ?? "N/A"}, " +
                                $"Total: ${order.TotalAmount:F2}, Items: {order.OrderLines?.Count ?? 0}");
            }
        }

        static void ViewOrderDetails(OrderService orderService)
        {
            Console.Write("Enter order ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var order = orderService.GetOrderById(id);
                if (order != null)
                {
                    Console.WriteLine("\n=== Order Details ===");
                    Console.WriteLine($"Order ID: {order.Id}");
                    Console.WriteLine($"Date: {order.Date:yyyy-MM-dd HH:mm}");
                    Console.WriteLine($"Customer: {order.Customer?.Name ?? "N/A"}");
                    Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
                    Console.WriteLine("\nOrder Items:");
                    foreach (var line in order.OrderLines ?? new List<OrderLine>())
                    {
                        Console.WriteLine($"  - {line.Product?.Name ?? "N/A"}: " +
                                        $"{line.Quantity} x ${line.UnitPrice:F2} = ${line.Quantity * line.UnitPrice:F2}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void CreateOrder(OrderService orderService, AppDbContext context)
        {
            Console.WriteLine("=== Create New Order ===");

            // Show customers
            var customers = context.Customers.ToList();
            if (customers.Count == 0)
            {
                Console.WriteLine("No customers available.");
                return;
            }

            Console.WriteLine("\nAvailable Customers:");
            foreach (var c in customers)
            {
                Console.WriteLine($"ID: {c.Id}, Name: {c.Name}");
            }

            Console.Write("\nEnter customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid customer ID.");
                return;
            }

            // Show products
            var products = context.Products.Include(p => p.Category).ToList();
            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
                return;
            }

            Console.WriteLine("\nAvailable Products:");
            foreach (var p in products)
            {
                Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Price: ${p.UnitPrice:F2}, Stock: {p.Stock}");
            }

            // Get order items
            var orderItems = new List<(int productId, int quantity)>();
            bool addingItems = true;

            while (addingItems)
            {
                Console.Write("\nEnter product ID (or 0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Invalid product ID.");
                    continue;
                }

                if (productId == 0)
                {
                    addingItems = false;
                    continue;
                }

                Console.Write("Enter quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                orderItems.Add((productId, quantity));
                Console.WriteLine("Item added to order.");
            }

            if (orderItems.Count == 0)
            {
                Console.WriteLine("No items added to order.");
                return;
            }

            orderService.CreateOrder(customerId, orderItems);
        }

        static void UpdateOrder(OrderService orderService, AppDbContext context)
        {
            Console.Write("Enter order ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var order = orderService.GetOrderById(orderId);
            if (order == null)
            {
                Console.WriteLine($"Order with ID {orderId} not found.");
                return;
            }

            Console.WriteLine($"\n=== Current Order Details ===");
            Console.WriteLine($"Order ID: {order.Id}");
            Console.WriteLine($"Customer: {order.Customer?.Name ?? "N/A"}");
            Console.WriteLine($"Date: {order.Date:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Total: ${order.TotalAmount:F2}");
            Console.WriteLine("\nOrder Items:");
            foreach (var line in order.OrderLines)
            {
                Console.WriteLine($"  - Product ID: {line.ProductId}, " +
                                $"Name: {line.Product?.Name ?? "N/A"}, " +
                                $"Quantity: {line.Quantity}, " +
                                $"Price: ${line.UnitPrice:F2}");
            }

            Console.WriteLine("\n=== Update Order Items ===");
            Console.Write("Enter product ID to add/update/remove: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID format.");
                return;
            }

            var product = context.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine($"Product with ID {productId} not found.");
                return;
            }

            Console.WriteLine($"\nProduct: {product.Name}");
            Console.WriteLine($"Current Stock: {product.Stock}");
            
            var existingLine = order.OrderLines.FirstOrDefault(ol => ol.ProductId == productId);
            if (existingLine != null)
            {
                Console.WriteLine($"Current quantity in order: {existingLine.Quantity}");
            }
            else
            {
                Console.WriteLine("This product is not in the order yet.");
            }

            Console.Write("Enter new quantity (0 to remove item): ");
            if (!int.TryParse(Console.ReadLine(), out int newQuantity))
            {
                Console.WriteLine("Invalid quantity format.");
                return;
            }

            if (newQuantity < 0)
            {
                Console.WriteLine("Quantity cannot be negative.");
                return;
            }

            Console.WriteLine("\n[UPDATE] Processing order update...");
            orderService.UpdateOrderItems(orderId, productId, newQuantity);
        }

        static void DeleteOrder(OrderService orderService)
        {
            Console.Write("Enter order ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                orderService.DeleteOrder(id);
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }

        static void ViewLowStockProducts(OrderService orderService)
        {
            Console.Write("Enter stock threshold (default 10): ");
            var input = Console.ReadLine();
            int threshold = 10;
            if (!string.IsNullOrWhiteSpace(input))
            {
                int.TryParse(input, out threshold);
            }

            var products = orderService.GetLowStockProducts(threshold);
            Console.WriteLine($"\n=== Low Stock Products ({products.Count}) ===");
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, " +
                                $"Stock: {product.Stock}, Category: {product.Category?.Name ?? "N/A"}");
            }
        }

        // Test DATABASE VIEW
        static void TestDatabaseView(OrderService orderService)
        {
            Console.WriteLine("\n=== Testing Database VIEW ===");
            Console.WriteLine("Querying OrderSummaryView...\n");
            
            var summaries = orderService.GetOrderSummariesFromView();
            
            if (summaries.Count == 0)
            {
                Console.WriteLine("No order summaries found.");
                return;
            }

            Console.WriteLine($"Found {summaries.Count} order summaries:\n");
            foreach (var summary in summaries)
            {
                Console.WriteLine($"Order ID: {summary.OrderId}");
                Console.WriteLine($"Date: {summary.OrderDate:yyyy-MM-dd}");
                Console.WriteLine($"Customer: {summary.CustomerName} ({summary.CustomerEmail})");
                Console.WriteLine($"Total Amount: ${summary.TotalAmount:F2}");
                Console.WriteLine($"Total Items: {summary.TotalItems}");
                Console.WriteLine("---");
            }
        }

        // Test ENCRYPTION
        static void TestEncryption(AppDbContext context)
        {
            Console.WriteLine("\n=== Testing Application-Level Encryption ===");
            Console.WriteLine("Customer addresses are stored ENCRYPTED in the database.\n");
            
            var customers = context.Customers.ToList();
            
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer: {customer.Name}");
                Console.WriteLine($"  Encrypted Address (in DB): {customer.Address}");
                Console.WriteLine($"  Decrypted Address: {OnlineShopping.Utilities.EncryptionHelper.Decrypt(customer.Address)}");
                Console.WriteLine();
            }

            // Demonstrate encryption
            Console.Write("Enter a test address to encrypt: ");
            var testAddress = Console.ReadLine() ?? "";
            
            if (!string.IsNullOrWhiteSpace(testAddress))
            {
                var encrypted = OnlineShopping.Utilities.EncryptionHelper.Encrypt(testAddress);
                var decrypted = OnlineShopping.Utilities.EncryptionHelper.Decrypt(encrypted);
                
                Console.WriteLine($"\nOriginal: {testAddress}");
                Console.WriteLine($"Encrypted: {encrypted}");
                Console.WriteLine($"Decrypted: {decrypted}");
            }
        }
    }
}
