using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Users
{
    public class SignInDTO
    {
        [Required]
        [CustomValidator.EmailPattern]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [CustomValidator.PasswordPattern]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
