using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class PricingTerm
    {
        public string Sku { get; set; }
        public string Term { get; set; }
        public PricingDimension PricingDimension { get; set; }
        public PricingDimension PricingDetail { get; set; }
        public string LeaseContractLength { get; set; }
        public string OfferingClass { get; set; }
        public string PurchaseOption { get; set; }
    }
}
