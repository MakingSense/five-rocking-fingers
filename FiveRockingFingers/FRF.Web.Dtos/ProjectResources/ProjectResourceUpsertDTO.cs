using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FRF.Web.Dtos.ProjectResources
{
    public class ProjectResourceUpsertDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProjectId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int ResourceId { get; set; }
        [Required]
        public int DedicatedHours { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
