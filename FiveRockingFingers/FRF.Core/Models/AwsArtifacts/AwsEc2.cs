using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FRF.Core.Models.AwsArtifacts
{
    public class AwsEc2 : Artifact
    {
        public const decimal PartialStorageDiscount = 0.5m;
        public const int PriceError = -1;        

        //Compute instance properties
        public int HoursUsedPerMonth => GetHoursUsedPerMonth();
        public string PurchaseOption => GetPurchaseOption();
        public decimal InstancePricePerUnit0 => GetInstancePricePerUnit0();
        public decimal InstancePricePerUnit1 => GetInstancePricePerUnit1();
        public string LeaseContractLength => GetLeaseContractLength();

        //EBS Storage properties
        public string VolumenApiName => GetVolumenApiName();
        public int NumberOfGbStorageInEbs => GetNumberOfGbStorageInEbs();
        public decimal EbsPricePerUnit => GetEbsPricePerUnit();

        //EBS Snapshots properties
        public int NumberOfGbChangedPerSnapshot => GetNumberOfGbChangedPerSnapshot();
        public decimal NumberOfSnapshotsPerMonth => GetNumberOfSnapshotsPerMonth();
        public decimal SnapshotPricePerUnit => GetSnapshotPricePerUnit();

        //EBS IOPS properties
        public int NumberOfIops => GetNumberOfIops();
        public decimal IopsPricePerUnit => GetIopsPricePerUnit();

        //EBS Throughput properties
        public int NumberOfMbpsThroughput => GetNumberOfMbpsThroughput();
        public decimal ThroughputPricePerUnit => GetThroughputPricePerUnit();

        //Data Transfer properties
        public List<DataTransferEc2> DataTransferItems => GetDataTransferItems();

        public AwsEc2(XElement settings)
        {
            Settings = settings;
            RelationalFields = new Dictionary<string, string>();
            RelationalFields.Add("hoursUsedPerMonth", SettingTypes.Decimal);
            RelationalFields.Add("numberOfGbStorageInEbs", SettingTypes.Decimal);
            RelationalFields.Add("numberOfSnapshotsPerMonth", SettingTypes.Decimal);
            RelationalFields.Add("numberOfGbChangedPerSnapshot", SettingTypes.Decimal);
            RelationalFields.Add("numberOfIopsPerMonth", SettingTypes.Decimal);
            RelationalFields.Add("numberOfMbpsThroughput", SettingTypes.Decimal);
            RelationalFields.Add("numberOfGbTransfer1", SettingTypes.Decimal);
        }

        override public decimal GetPrice()
        {
            if(!ArePricesCorrect())
            {
                return PriceError;
            }

            var totalPrice = GetInstancePrice() + GetEbsPrice() + GetSnapshotsPrice()
                + GetIntraRegionDataTransferPrice() + GetIopsPrice() + GetThroughputPrice();

            return totalPrice;
        }

        private decimal GetIntraRegionDataTransferPrice()
        {
            var intraRegionDataTransferPrice = 0m;

            foreach(var dataTransferItem in DataTransferItems)
            {
                if(dataTransferItem.TransferType.Equals(AwsEc2Descriptions.IntraRegionDataTransfer))
                {
                    intraRegionDataTransferPrice += 2 * dataTransferItem.NumberOfGbTransferIntraRegion * dataTransferItem.IntraTegionDataTransferPricePerUnit;
                }
                else
                {
                    intraRegionDataTransferPrice += dataTransferItem.NumberOfGbTransferIntraRegion * dataTransferItem.IntraTegionDataTransferPricePerUnit;
                }
            }

            return intraRegionDataTransferPrice;
        }

        private decimal GetIopsPrice()
        {
            var price = 0m;

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3Value))
            {
                var numberOfIopsBillable = NumberOfIops - AwsEc2Descriptions.FreeTierGp3Iops;

                if(numberOfIopsBillable > 0)
                {
                    price = numberOfIopsBillable * IopsPricePerUnit;
                }

                return price;
            }

            if(VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1Value) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2Value))
            {
                price = NumberOfIops * IopsPricePerUnit;

                return price;
            }

            return price;
        }

        private decimal GetThroughputPrice()
        {
            var price = 0m;

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameGp3Value))
            {
                var numberOfMbpsThroughputBillable = NumberOfMbpsThroughput - AwsEc2Descriptions.FreeTierGp3Throughput;

                var numberOfGbpsThroughputBillable = numberOfMbpsThroughputBillable / 1024;

                if (numberOfMbpsThroughputBillable > 0)
                {
                    price = numberOfGbpsThroughputBillable * ThroughputPricePerUnit;
                }

                return price;
            }

            if (VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo1Value) || VolumenApiName.Equals(AwsEc2Descriptions.VolumenApiNameIo2Value))
            {
                price = NumberOfMbpsThroughput * ThroughputPricePerUnit;

                return price;
            }

            return price;
        }

        private decimal GetSnapshotsPrice()
        {
            var initialSnapshotPrice = NumberOfGbStorageInEbs * SnapshotPricePerUnit;

            var incrementaSnapshotsPrice = NumberOfSnapshotsPerMonth * SnapshotPricePerUnit * PartialStorageDiscount * NumberOfGbChangedPerSnapshot;

            var totalShanpshotPrice = initialSnapshotPrice + incrementaSnapshotsPrice;

            return totalShanpshotPrice;
        }

        private decimal GetEbsPrice()
        {
            return NumberOfGbStorageInEbs * EbsPricePerUnit;
        }

        private decimal GetInstancePrice()
        {
            var price = 0m;

            if (PurchaseOption.Equals(AwsEc2Descriptions.AllUpfront, StringComparison.InvariantCultureIgnoreCase) &&
                LeaseContractLength.Equals(AwsEc2Descriptions.OneYear, StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPriceAllUpfront1Year();
            }

            if (PurchaseOption.Equals(AwsEc2Descriptions.AllUpfront, StringComparison.InvariantCultureIgnoreCase) &&
                LeaseContractLength.Equals(AwsEc2Descriptions.ThreeYears, StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPriceAllUpfront3Year();
            }

            if (PurchaseOption.Equals(AwsEc2Descriptions.PartialUpfront, StringComparison.InvariantCultureIgnoreCase) &&
                LeaseContractLength.Equals(AwsEc2Descriptions.OneYear, StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPricePartialUpfront1Year();
            }

            if (PurchaseOption.Equals(AwsEc2Descriptions.PartialUpfront, StringComparison.InvariantCultureIgnoreCase) &&
                LeaseContractLength.Equals(AwsEc2Descriptions.ThreeYears, StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPricePartialUpfront3Year();
            }

            if (PurchaseOption.Equals(AwsEc2Descriptions.NoUpfront, StringComparison.InvariantCultureIgnoreCase))
            {
                price = GetPriceNoUpfront();
            }

            return price;
        }

        private decimal GetPriceAllUpfront1Year()
        {
            return InstancePricePerUnit1 / 12;
        }

        private decimal GetPriceAllUpfront3Year()
        {
            return InstancePricePerUnit1 / 36;
        }

        private decimal GetPricePartialUpfront1Year()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth + InstancePricePerUnit1 / 12;
        }

        private decimal GetPricePartialUpfront3Year()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth + InstancePricePerUnit1 / 36;
        }

        private decimal GetPriceNoUpfront()
        {
            return InstancePricePerUnit0 * HoursUsedPerMonth;
        }

        private int GetHoursUsedPerMonth()
        {
            var hoursUsedPerMonth = int.Parse(Settings.Element(AwsEc2Descriptions.HoursUsedPerMonth).Value);

            return hoursUsedPerMonth;
        }

        private decimal GetInstancePricePerUnit0()
        {
            var instancePricePerUnit0 = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product0)
                .Element(AwsEc2Descriptions.PricingDimensions)
                .Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit)
                .Value);

            return instancePricePerUnit0;
        }

        private decimal GetInstancePricePerUnit1()
        {
            var instancePricePerUnit1 = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product0)
                .Element(AwsEc2Descriptions.PricingDimensions).Element(AwsEc2Descriptions.Range1)
                .Element(AwsEc2Descriptions.PricePerUnit)
                .Value);

            return instancePricePerUnit1;
        }

        private string GetPurchaseOption()
        {
            var purchaseOption = Settings.Element(AwsEc2Descriptions.Product0).Element(AwsEc2Descriptions.PurchaseOption).Value;

            purchaseOption = purchaseOption.Replace(" ", "");

            return purchaseOption;
        }

        private string GetLeaseContractLength()
        {
            var leaseContractLength = Settings.Element(AwsEc2Descriptions.Product0).Element(AwsEc2Descriptions.LeaseContractLength).Value;

            leaseContractLength = leaseContractLength.Replace(" ", "");

            return leaseContractLength;
        }

        private string GetVolumenApiName()
        {
            var volumenApiName = Settings.Element(AwsEc2Descriptions.Product1).Element(AwsEc2Descriptions.VolumeApiName).Value;

            return volumenApiName;
        }

        private int GetNumberOfGbStorageInEbs()
        {
            var numberOfGbStorageInEbs = int.Parse(Settings.Element(AwsEc2Descriptions.NumberOfGbStorageInEbs).Value);

            return numberOfGbStorageInEbs;
        }

        private decimal GetEbsPricePerUnit()
        {
            var ebsPricePerUnit = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product1)
                .Element(AwsEc2Descriptions.PricingDimensions).Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit).Value);

            return ebsPricePerUnit;
        }

        private decimal GetNumberOfSnapshotsPerMonth()
        {
            var numberOfSnapshotsPerMonth = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.NumberOfSnapshotsPerMonth).Value);

            return numberOfSnapshotsPerMonth;
        }

        private int GetNumberOfGbChangedPerSnapshot()
        {
            var numberOfGbChangedPerSnapshot = int.Parse(Settings.Element(AwsEc2Descriptions.NumberOfGbChangedPerSnapshot).Value);

            return numberOfGbChangedPerSnapshot;
        }

        private decimal GetSnapshotPricePerUnit()
        {
            var snapshotPricePerUnit = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product2)
                .Element(AwsEc2Descriptions.PricingDimensions)
                .Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit).Value);

            return snapshotPricePerUnit;
        }

        private int GetNumberOfIops()
        {
            var numberOfIops = 0;

            if (Settings.Element(AwsEc2Descriptions.NumberOfIopsPerMonth) != null)
            {
                if (int.TryParse(Settings.Element(AwsEc2Descriptions.NumberOfIopsPerMonth).Value, out int numberOfIopsParse))
                {
                    numberOfIops = numberOfIopsParse;
                }
            }

            return numberOfIops;
        }

        private decimal GetIopsPricePerUnit()
        {
            var iopsPricePerUnit = 0m;

            if (Settings.Element(AwsEc2Descriptions.Product3) != null &&
                Settings.Element(AwsEc2Descriptions.Product3)
                .Element(AwsEc2Descriptions.PricingDimensions)
                .Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit) != null)
            {
                iopsPricePerUnit = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product3)
                    .Element(AwsEc2Descriptions.PricingDimensions)
                    .Element(AwsEc2Descriptions.Range0)
                    .Element(AwsEc2Descriptions.PricePerUnit).Value);
            }

            return iopsPricePerUnit;
        }

        private int GetNumberOfMbpsThroughput()
        {
            var numberOfMbpsThroughput = 0;

            if (Settings.Element(AwsEc2Descriptions.NumberOfMbpsThroughput) != null)
            {
                if (int.TryParse(Settings.Element(AwsEc2Descriptions.NumberOfMbpsThroughput).Value, out int numberOfMbpsThroughputParse))
                {
                    numberOfMbpsThroughput = numberOfMbpsThroughputParse;
                }
            }

            return numberOfMbpsThroughput;
        }

        private decimal GetThroughputPricePerUnit()
        {
            var throughputPricePerUnit = 0m;

            if (Settings.Element(AwsEc2Descriptions.Product4) != null &&
                Settings.Element(AwsEc2Descriptions.Product4)
                .Element(AwsEc2Descriptions.PricingDimensions)
                .Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit) != null)
            {
                throughputPricePerUnit = GetDecimalPrice(Settings.Element(AwsEc2Descriptions.Product4)
                    .Element(AwsEc2Descriptions.PricingDimensions)
                    .Element(AwsEc2Descriptions.Range0)
                    .Element(AwsEc2Descriptions.PricePerUnit)
                    .Value);
            }

            return throughputPricePerUnit;
        }

        private List<DataTransferEc2> GetDataTransferItems()
        {
            var dataTransferItems = new List<DataTransferEc2>();

            for(var i = 0; i < 3; i++)
            {
                var dataTransferNumberOfGbNode = Settings.Element($"numberOfGbTransfer{i}");
                var dataTransferPricingNode = Settings.Element($"product5-{i}");

                if(dataTransferNumberOfGbNode == null || dataTransferPricingNode == null)
                {
                    continue;
                }

                var transferType = dataTransferPricingNode.Element(AwsEc2Descriptions.TransferType).Value;
                var numberOfGbTransfer = int.Parse(dataTransferNumberOfGbNode.Value);
                var dataTransferPricePerUnit = GetDecimalPrice(dataTransferPricingNode
                .Element(AwsEc2Descriptions.PricingDimensions)
                .Element(AwsEc2Descriptions.Range0)
                .Element(AwsEc2Descriptions.PricePerUnit)
                .Value);

                var dataTransferItem = new DataTransferEc2
                {
                    TransferType = transferType,
                    NumberOfGbTransferIntraRegion = numberOfGbTransfer,
                    IntraTegionDataTransferPricePerUnit = dataTransferPricePerUnit
                };

                dataTransferItems.Add(dataTransferItem);
            }

            return dataTransferItems;
        }

        private bool ArePricesCorrect()
        {
            return PricesInDataTransferItemsAreCorrect() &&
                ThroughputPricePerUnit != PriceError &&
                IopsPricePerUnit != PriceError &&
                SnapshotPricePerUnit != PriceError &&
                EbsPricePerUnit != PriceError &&
                InstancePricePerUnit0 != PriceError &&
                InstancePricePerUnit1 != PriceError;
        }

        private bool PricesInDataTransferItemsAreCorrect()
        {
            var flag = true;

            foreach(var dataTransferItem in DataTransferItems)
            {
                if(dataTransferItem.IntraTegionDataTransferPricePerUnit == PriceError)
                {
                    flag = false;
                }
            }

            return flag;
        }

        private decimal GetDecimalPrice(string pricePerUnit)
        {
            if (decimal.TryParse(pricePerUnit, NumberStyles.AllowExponent, CultureInfo.InvariantCulture,
                out decimal decimalPrice)) return decimalPrice;

            if (decimal.TryParse(pricePerUnit, NumberStyles.Currency, CultureInfo.InvariantCulture,
                out decimalPrice)) return decimalPrice;

            return -1;
        }
    }
}
