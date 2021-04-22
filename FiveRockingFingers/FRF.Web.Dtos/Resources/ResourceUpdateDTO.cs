using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Resources
{
    public class ResourceUpdateDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(1,100000000000, ErrorMessage = "Salary has to be greater than {1}")]
        public decimal SalaryPerMonth { get; set; }
        [Required]
        public int WorkloadCapacity { get; set; }
    }
}