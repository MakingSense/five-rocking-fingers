using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Users
{
    public class UserPublicProfileDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}