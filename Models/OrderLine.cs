namespace OnlineShopping.Models

{
    public class OrderLine
    {
        public int Id {get; set;} //PK
        public int ProductId {get; set; } //FK
        public int OrderId {get; set; } //FK
        public decimal UnitPrice {get; set; }
        public int Quantity {get; set; }

        public Order? Order {get; set;} // Nav property for one side
        public Product? Product {get; set; }
    }
}