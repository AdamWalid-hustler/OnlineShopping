using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models

{
    public class Customer
    {
        public int CustomerId {get; set; } //PK
        public string Name {get; set; } = "";
        public required string Email {get; set;}
        public string Adress {get; set; } = "";

        public List<Order> Orders {get; set; } new();

        
    }
}