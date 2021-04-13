using System;

namespace FRF.Core.Models
{
    public class ProjectResources
    {
        public int Id { get; set; }
        public int IdProject { get; set; }
        public Project Project { get; set; }
        public int IdResource { get; set; }
        public Resources Resources { get; set; }
        public int DedicatedHours { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}