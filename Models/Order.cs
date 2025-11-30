
namespace OnlineShopping.Models

{
    public class Order
    {
        public int OrderId {get; set; } //PK
        public int CustomerId {get; set; } //FK
        DateTime Date {get; set; }
        public decimal TotalAmount {get; set; }
    }
}