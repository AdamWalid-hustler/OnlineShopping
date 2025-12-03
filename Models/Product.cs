namespace OnlineShopping.Models

{
    public class Product
    {
        public int Id {get; set; } //PK
        public int CategoryId {get; set; } //FK
        public string Name {get; set; } = "";
        public decimal UnitPrice {get; set; }

        public List<OrderLine> OrderLines {get; set; } = new List<OrderLine>(); // A product can appear in many OrderLines
    }
}