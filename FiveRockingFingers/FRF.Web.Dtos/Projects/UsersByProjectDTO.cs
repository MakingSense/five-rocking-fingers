using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
