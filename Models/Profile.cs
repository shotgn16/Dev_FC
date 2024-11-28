using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ForestChurches.Models
{
    public class Profile
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [AllowNull]
        [DataType(DataType.MultilineText)]
        public List<string> Roles { get; set; }
    }
}
