
namespace OnlineShopping.Models

{
    public class Order
    {
        public int Id {get; set; } //PK
        public int CustomerId {get; set; } //FK
        public DateTime Date {get; set; }
        public decimal TotalAmount {get; set; }
        public Customer? Customer {get; set; } // Navigation property for the one side

        public List<OrderLine> OrderLines {get; set; } = new List<OrderLine>(); // An order can appear in many OrderLines
    
    }
}