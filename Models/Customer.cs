using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Models
{
    public class Customer
    {
        public int Id { get; set; } // PK
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = "";
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = "";
        
        [Required(ErrorMessage = "Password hash is required")]
        public string PasswordHash { get; set; } = "";
        
        [Required(ErrorMessage = "Password salt is required")]
        public string PasswordSalt { get; set; } = "";

        // Navigation property
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}