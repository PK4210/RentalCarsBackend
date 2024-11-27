using System.ComponentModel.DataAnnotations;

namespace RentalCars.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool UserType { get; set; } // true: Admin, false: Regular user
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
