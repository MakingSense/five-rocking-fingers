using System;

namespace FRF.Core.Models
{
    public class User
    {
        public string Name { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserSignIn
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}