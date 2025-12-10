using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Product
    {
        public int Id { get; set; } // PK
        
        public int CategoryId { get; set; } // FK
        
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
        public string Name { get; set; } = "";
        
        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Unit price must be between 0.01 and 999999.99")]
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public Category? Category { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}