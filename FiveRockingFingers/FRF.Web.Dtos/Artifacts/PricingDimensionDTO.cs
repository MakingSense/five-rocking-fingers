using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Artifacts
{
    public class PricingDimensionDTO
    {
        public string Unit { get; set; }
        public string EndRange { get; set; }
        public string Description { get; set; }
        public string RateCode { get; set; }
        public string BeginRange { get; set; }
        public string Currency { get; set; }
        public float PricePerUnit { get; set; }
    }
}
