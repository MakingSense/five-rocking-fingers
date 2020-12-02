using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class UserProfileUpsert
    {
        [Required] 
        public Guid? UserId { get; set; }
    }
}