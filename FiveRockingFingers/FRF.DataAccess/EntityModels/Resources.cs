﻿namespace FRF.DataAccess.EntityModels
{
    public class Resources
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
        public decimal SalaryPerMonth { get; set; }
        public int WorkloadCapacity { get; set; }
    }
}