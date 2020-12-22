using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class UserProfileUpsertDTO
    {
        [Required] 
        public Guid? UserId { get; set; }
    }
}