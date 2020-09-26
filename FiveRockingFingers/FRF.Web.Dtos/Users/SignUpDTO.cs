using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Users
{
    public class SignUpDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string FamilyName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")]
        public string Password { get; set; }
    }
}
