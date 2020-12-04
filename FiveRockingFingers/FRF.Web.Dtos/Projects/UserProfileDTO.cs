using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class UserProfileDTO
    {
        public Guid? UserId { get; set; }
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
    }
}
