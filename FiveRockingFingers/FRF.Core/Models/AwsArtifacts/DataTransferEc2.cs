using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models.AwsArtifacts
{
    public class DataTransferEc2
    {
        public string TransferType { get; set; }
        public int NumberOfGbTransferIntraRegion { get; set; }
        public decimal IntraTegionDataTransferPricePerUnit { get; set; }
    }
}
