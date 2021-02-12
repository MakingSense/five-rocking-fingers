﻿namespace FRF.Core.Models
{
    public class PricingDimension
    {
        public string Unit { get; set; }
        public float EndRange { get; set; }
        public string Description { get; set; }
        public string RateCode { get; set; }
        public float BeginRange { get; set; }
        public string Currency { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}
