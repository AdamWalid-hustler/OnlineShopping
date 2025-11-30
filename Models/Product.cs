namespace OnlineShopping.Models

{
    public class Product
    {
        public int ProductId {get; set; } //PK
        public int CategoryId {get; set; } //FK
        public int Name {get; set; }
        public decimal UnitPrice {get; set; }
    }
}