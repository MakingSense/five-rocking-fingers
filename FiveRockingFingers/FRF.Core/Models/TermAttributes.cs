using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class TermAttributes : IEquatable<TermAttributes>
    {
        public string TermType { get; set; }
        public string LeaseContractLength { get; set; }
        public string OfferingClass { get; set; }
        public string PurchaseOption { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TermAttributes);
        }

        public bool Equals(TermAttributes other)
        {
            return other != null &&
                   TermType == other.TermType &&
                   LeaseContractLength == other.LeaseContractLength &&
                   OfferingClass == other.OfferingClass &&
                   PurchaseOption == other.PurchaseOption;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TermType, LeaseContractLength, OfferingClass, PurchaseOption);
        }
    }
}
