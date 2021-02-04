using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Artifacts
{
    public class PricingTermDTO
    {
        public string Sku { get; set; }
        public string Term { get; set; }
        public List<PricingDimensionDTO> PricingDimensions { get; set; }
        public string LeaseContractLength { get; set; }
        public string OfferingClass { get; set; }
        public string PurchaseOption { get; set; }
    }
}
