using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Users
{
    public class SignUpDTO
    {
        [Required] public string Name { get; set; }
        [Required] public string FamilyName { get; set; }

        [Required]
        [CustomValidator.EmailPattern]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [CustomValidator.PasswordPattern]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}