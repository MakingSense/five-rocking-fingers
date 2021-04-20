using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.ProjectResources
{
    public class ProjectResourceDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public ProjectDTO Project { get; set; }
        public int ResourceId { get; set; }
        public int DedicatedHours { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
