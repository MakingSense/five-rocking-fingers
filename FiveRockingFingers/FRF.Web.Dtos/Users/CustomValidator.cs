using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace FRF.Web.Dtos.Users
{
    public class CustomValidator
    {
        public class EmailPatternAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var configuration = (IConfiguration) validationContext
                    .GetService(typeof(IConfiguration));

                if (string.IsNullOrWhiteSpace(value as string)) return new ValidationResult("Should not be empty");

                var regexEmailPattern = new Regex(configuration["Regex:EmailPattern"]);
                
                if (regexEmailPattern.IsMatch((string) value)) return ValidationResult.Success;

                return new ValidationResult("Is not a correct email");
            }
        }

        public class PasswordPatternAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var configuration = (IConfiguration) validationContext
                    .GetService(typeof(IConfiguration));

                if (string.IsNullOrWhiteSpace(value as string)) return new ValidationResult("Should not be empty");

                var regexPasswordPattern = new Regex(configuration["Regex:PasswordPattern"]);
                
                if (regexPasswordPattern.IsMatch((string) value)) return ValidationResult.Success;

                return new ValidationResult("Is not a correct password format");
            }
        }
    }
}