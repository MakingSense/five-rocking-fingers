using System;

namespace FRF.DataAccess.EntityModels
{
    public class ProjectResource
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public int DedicatedHours { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}