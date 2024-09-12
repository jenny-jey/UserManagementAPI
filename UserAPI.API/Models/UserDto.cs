using System.ComponentModel.DataAnnotations;

namespace UserAPI.API.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; } = string.Empty;
        [Phone]
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
