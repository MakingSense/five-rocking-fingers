using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class UsersProfile
    {
        public string? Fullname { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public Guid UserId { get; set; }
    }
}