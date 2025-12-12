using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

namespace OnlineShopping.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE Order with OrderLines and inventory update
        // Uses TRANSACTION to ensure data consistency
        public Order? CreateOrder(int customerId, List<(int productId, int quantity)> orderItems)
        {
            // Start a database transaction
            using var transaction = _context.Database.BeginTransaction();
            
            try
            {
                Console.WriteLine("[TRANSACTION] Starting order creation...");
                
                // Check if customer exists
                var customer = _context.Customers.Find(customerId);
                if (customer == null)
                {
                    Console.WriteLine("Error: Customer not found");
                    Console.WriteLine("[TRANSACTION] Rolling back - no changes saved");
                    transaction.Rollback();
                    return null;
                }

                // Validate all products and check stock
                foreach (var item in orderItems)
                {
                    var product = _context.Products.Find(item.productId);
                    if (product == null)
                    {
                        Console.WriteLine($"Error: Product ID {item.productId} not found");
                        Console.WriteLine("[TRANSACTION] Rolling back - no changes saved");
                        transaction.Rollback();
                        return null;
                    }

                    if (product.Stock < item.quantity)
                    {
                        Console.WriteLine($"Error: Not enough stock for {product.Name}. Available: {product.Stock}, Requested: {item.quantity}");
                        Console.WriteLine("[TRANSACTION] Rolling back - no changes saved");
                        transaction.Rollback();
                        return null;
                    }

                    if (item.quantity <= 0)
                    {
                        Console.WriteLine("Error: Quantity must be greater than 0");
                        Console.WriteLine("[TRANSACTION] Rolling back - no changes saved");
                        transaction.Rollback();
                        return null;
                    }
                }

                // Create order
                var order = new Order
                {
                    CustomerId = customerId,
                    Date = DateTime.Now,
                    TotalAmount = 0
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
                Console.WriteLine("[TRANSACTION] Order created in database");

                // Create order lines and update inventory
                decimal totalAmount = 0;
                foreach (var item in orderItems)
                {
                    var product = _context.Products.Find(item.productId);
                    if (product == null) continue;
                    
                    var orderLine = new OrderLine
                    {
                        OrderId = order.Id,
                        ProductId = item.productId,
                        UnitPrice = product.UnitPrice,
                        Quantity = item.quantity
                    };

                    _context.OrderLines.Add(orderLine);
                    
                    // Reduce stock (inventory update)
                    Console.WriteLine($"[TRANSACTION] Reducing stock for {product.Name}: {product.Stock} -> {product.Stock - item.quantity}");
                    product.Stock -= item.quantity;
                    
                    totalAmount += product.UnitPrice * item.quantity;
                }

                // Update order total
                order.TotalAmount = totalAmount;
                _context.SaveChanges();
                
                // Commit transaction - all changes are saved
                transaction.Commit();
                Console.WriteLine("[TRANSACTION] Committed - all changes saved successfully");
                Console.WriteLine($"Order created successfully. Total: ${totalAmount:F2}");
                return order;
            }
            catch (Exception ex)
            {
                // If any error occurs, 
                Console.WriteLine($"Error creating order: {ex.Message}");
                Console.WriteLine("[TRANSACTION] Rolling back - no changes saved");
                transaction.Rollback();
                return null;
            }
        }

        // READ Single Order with details
        public Order? GetOrderById(int id)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderLines)
                        .ThenInclude(ol => ol.Product)
                    .FirstOrDefault(o => o.Id == id);

                if (order == null)
                {
                    Console.WriteLine("Error: Order not found");
                }

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order: {ex.Message}");
                return null;
            }
        }

        // READ All Orders
        public List<Order> GetAllOrders()
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderLines)
                    .OrderByDescending(o => o.Date)
                    .ToList();

                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving orders: {ex.Message}");
                return new List<Order>();
            }
        }

        // READ Orders with PAGINATION and SORTING
        public List<Order> GetOrdersPaged(int pageNumber, int pageSize, string sortBy = "Date", bool descending = true)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderLines)
                    .AsQueryable();

                // Sorting on multiple columns
                if (sortBy == "Date")
                {
                    query = descending ? query.OrderByDescending(o => o.Date) : query.OrderBy(o => o.Date);
                }
                else if (sortBy == "TotalAmount")
                {
                    query = descending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount);
                }
                else if (sortBy == "Customer")
                {
                    query = descending ? query.OrderByDescending(o => o.Customer!.Name) : query.OrderBy(o => o.Customer!.Name);
                }
                else
                {
                    // Default sort by Date
                    query = descending ? query.OrderByDescending(o => o.Date) : query.OrderBy(o => o.Date);
                }

                // Pagination
                var orders = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                Console.WriteLine($"Retrieved page {pageNumber} with {orders.Count} orders (sorted by {sortBy})");
                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving orders: {ex.Message}");
                return new List<Order>();
            }
        }

        // Get total count for pagination
        public int GetOrderCount()
        {
            return _context.Orders.Count();
        }

        // Get order summaries using DATABASE VIEW
        public List<OrderSummaryView> GetOrderSummariesFromView()
        {
            try
            {
                // Query the database view
                var summaries = _context.OrderSummaryView.ToList();
                Console.WriteLine($"Retrieved {summaries.Count} order summaries from database VIEW");
                return summaries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order summaries: {ex.Message}");
                return new List<OrderSummaryView>();
            }
        }

        // UPDATE Order, Add/Remove items with inventory updates
        public bool UpdateOrderItems(int orderId, int productId, int newQuantity)
        {
            using var transaction = _context.Database.BeginTransaction();
            
            try
            {
                Console.WriteLine("[TRANSACTION] Starting order update...");
                
                var order = _context.Orders
                    .Include(o => o.OrderLines)
                    .FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                {
                    Console.WriteLine("Error: Order not found");
                    transaction.Rollback();
                    return false;
                }

                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    Console.WriteLine("Error: Product not found");
                    transaction.Rollback();
                    return false;
                }

                var orderLine = order.OrderLines.FirstOrDefault(ol => ol.ProductId == productId);

                if (newQuantity <= 0)
                {
                    // Remove item from order
                    if (orderLine != null)
                    {
                        Console.WriteLine($"[INVENTORY] Restoring stock for {product.Name}: {product.Stock} -> {product.Stock + orderLine.Quantity}");
                        product.Stock += orderLine.Quantity;
                        _context.OrderLines.Remove(orderLine);
                    }
                }
                else
                {
                    if (orderLine == null)
                    {
                        // Add new item to order
                        if (product.Stock < newQuantity)
                        {
                            Console.WriteLine($"Error: Not enough stock. Available: {product.Stock}");
                            transaction.Rollback();
                            return false;
                        }

                        orderLine = new OrderLine
                        {
                            OrderId = orderId,
                            ProductId = productId,
                            UnitPrice = product.UnitPrice,
                            Quantity = newQuantity
                        };
                        _context.OrderLines.Add(orderLine);
                        
                        Console.WriteLine($"[INVENTORY] Reducing stock for {product.Name}: {product.Stock} -> {product.Stock - newQuantity}");
                        product.Stock -= newQuantity;
                    }
                    else
                    {
                        // Update existing item quantity
                        int quantityDiff = newQuantity - orderLine.Quantity;
                        
                        if (quantityDiff > 0 && product.Stock < quantityDiff)
                        {
                            Console.WriteLine($"Error: Not enough stock. Available: {product.Stock}, Need: {quantityDiff}");
                            transaction.Rollback();
                            return false;
                        }

                        Console.WriteLine($"[INVENTORY] Adjusting stock for {product.Name}: {product.Stock} -> {product.Stock - quantityDiff}");
                        product.Stock -= quantityDiff;
                        orderLine.Quantity = newQuantity;
                    }
                }

                // Recalculate order total
                order.TotalAmount = order.OrderLines.Sum(ol => ol.Quantity * ol.UnitPrice);
                _context.SaveChanges();
                
                transaction.Commit();
                Console.WriteLine("[TRANSACTION] Order updated successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order: {ex.Message}");
                transaction.Rollback();
                return false;
            }
        }

        // DELETE Order and restore inventory
        public bool DeleteOrder(int id)
        {
            try
            {
                var order = _context.Orders
                    .Include(o => o.OrderLines)
                        .ThenInclude(ol => ol.Product)
                    .FirstOrDefault(o => o.Id == id);

                if (order == null)
                {
                    Console.WriteLine("Error: Order not found");
                    return false;
                }

                // Restore inventory before deleting
                foreach (var orderLine in order.OrderLines)
                {
                    if (orderLine.Product != null)
                    {
                        orderLine.Product.Stock += orderLine.Quantity;
                    }
                }

                // Remove order lines
                _context.OrderLines.RemoveRange(order.OrderLines);
                
                // Remove order
                _context.Orders.Remove(order);
                _context.SaveChanges();

                Console.WriteLine("Order deleted successfully and inventory restored");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting order: {ex.Message}");
                return false;
            }
        }

        // Get customer order history
        public List<Order> GetCustomerOrders(int customerId)
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.OrderLines)
                        .ThenInclude(ol => ol.Product)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.Date)
                    .ToList();

                return orders;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving orders: {ex.Message}");
                return new List<Order>();
            }
        }

        // Get low stock products 
        public List<Product> GetLowStockProducts(int threshold = 10)
        {
            try
            {
                var products = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Stock <= threshold)
                    .OrderBy(p => p.Stock)
                    .ToList();

                return products;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving low stock products: {ex.Message}");
                return new List<Product>();
            }
        }
    }
}
