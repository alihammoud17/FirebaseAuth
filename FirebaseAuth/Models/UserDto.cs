using System.ComponentModel.DataAnnotations;

namespace FirebaseAuth.Models
{
    public class UserDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
