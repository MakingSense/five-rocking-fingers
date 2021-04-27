﻿using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Resources
{
    public class ResourceUpsertDTO
    {
        [Required]
        public string RoleName { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(1,100000000000, ErrorMessage = "Salary has to be {1} or greater")]
        public decimal SalaryPerMonth { get; set; }
        [Required]
        public int WorkloadCapacity { get; set; }
    }
}