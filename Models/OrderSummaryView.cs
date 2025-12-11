namespace OnlineShopping.Models
{
    // Database VIEW - Order Summary
    // This view combines order data with customer and product information
    public class OrderSummaryView
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
