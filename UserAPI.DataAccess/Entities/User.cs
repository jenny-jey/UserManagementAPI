using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.DataAccess
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string EmailAddress { get; set; } = string.Empty;
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        //[Required]
        //public string PasswordHash { get; set; } = string.Empty;

        //public DateTime? DateOfBirth { get; set; }
        //[Required]
        //public string Role { get; set; } = string.Empty;

        //public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        //public DateTime? UpdatedDate { get; set; }
    }
}
