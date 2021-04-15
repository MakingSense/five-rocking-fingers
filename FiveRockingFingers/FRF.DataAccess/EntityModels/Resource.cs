using System.Collections.Generic;

namespace FRF.DataAccess.EntityModels
{
    public class Resource
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public int WorkloadCapacity { get; set; }
        public IList<ProjectResource> ProjectResource { get; set; }
    }
}