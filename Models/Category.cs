using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Category
    {
        public int Id { get; set; } // PK
        
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 50 characters")]
        public string Name { get; set; } = "";
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = "";

        // Navigation property
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
