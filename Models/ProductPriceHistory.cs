namespace OnlineShopping.Models
{
    public class ProductPriceHistory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangeType { get; set; } = string.Empty; // "UPDATE" or "INSERT"
    }
}
