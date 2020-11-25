using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class UsersByProjectDTO
    {
        [Required]
        public Guid? UserId { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }
}
