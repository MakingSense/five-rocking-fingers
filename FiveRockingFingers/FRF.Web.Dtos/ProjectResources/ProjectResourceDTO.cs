using FRF.Web.Dtos.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.ProjectResources
{
    public class ProjectResourceDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ResourceId { get; set; }
        public ResourceDTO Resource { get; set; }
        public int DedicatedHours { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
