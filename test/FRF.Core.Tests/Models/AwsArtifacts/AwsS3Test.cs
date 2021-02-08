using System.Xml.Linq;
using FRF.Core.Models.AwsArtifacts;
using Newtonsoft.Json;
using Xunit;

namespace FRF.Core.Tests.Models
{
    public class AwsS3Test
    {
        private readonly AwsS3 _classUnderTest;

        public AwsS3Test()
        {
            _classUnderTest = new AwsS3();
        }

        [Fact]
        public void GetPrice_WhenIsIntelligentTiering_ReturnDecimalPrice()
        {
            // Arrange
            var writeRequestsUsed = 2000;
            var retrieveRequestsUsed = 3000;
            var storageUsed = 1024;
            var infrequentAccessMultiplier = 42;
            var standardPricePerUnitTier1 = "0.023";
            var standardPricePerUnitTier2 = "0.021";
            var standardPricePerUnitTier3 = "0.0201";
            var infrequentPricePerUnit = "5E-06";
            var writeRequestsPrice = "5E-06";
            var retrieveRequestsPrice = "4E-07";

            const decimal FinalCost = 17.3249920m;

            _classUnderTest.Settings = RetrieveIntelligentTieringSettings(writeRequestsUsed, retrieveRequestsUsed, storageUsed,
                infrequentAccessMultiplier, standardPricePerUnitTier1
                , standardPricePerUnitTier2, standardPricePerUnitTier3, infrequentPricePerUnit, writeRequestsPrice,
                retrieveRequestsPrice);
            _classUnderTest.StorageUsed = storageUsed;
            _classUnderTest.RetrieveRequestsUsed = retrieveRequestsUsed;
            _classUnderTest.WriteRequestsUsed = writeRequestsUsed;
            _classUnderTest.InfrequentAccessMultiplier = infrequentAccessMultiplier;
            
            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost,result);
        }

        [Fact]
        public void GetPrice_WhenIsStandardService_ReturnDecimalPrice()
        {
            // Arrange
            var writeRequestsUsed = 2000;
            var retrieveRequestsUsed = 3000;
            var storageUsed = 1024;
            var standardPricePerUnitTier1 = "0.023";
            var standardPricePerUnitTier2 = "0.021";
            var standardPricePerUnitTier3 = "0.022";
            var writeRequestsPrice = "4E-07";
            var retrieveRequestsPrice = "5E-06";
            var endRange1 = 51200;

            const decimal FinalCost = 23.5678000M;

            _classUnderTest.Settings = RetrieveSettings(writeRequestsUsed, retrieveRequestsUsed, storageUsed, standardPricePerUnitTier1
                , standardPricePerUnitTier2, standardPricePerUnitTier3, writeRequestsPrice,
                retrieveRequestsPrice, endRange1);
            _classUnderTest.StorageUsed = storageUsed;
            _classUnderTest.RetrieveRequestsUsed = retrieveRequestsUsed;
            _classUnderTest.WriteRequestsUsed = writeRequestsUsed;

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }

        [Fact]
        public void GetPrice_StandardServiceWhenInvalidEndRange_Return0()
        {
            // Arrange
            var writeRequestsUsed = 2000;
            var retrieveRequestsUsed = 3000;
            var storageUsed = 1024;
            var standardPricePerUnitTier1 = "0.023";
            var standardPricePerUnitTier2 = "0.021";
            var standardPricePerUnitTier3 = "0.022";
            var writeRequestsPrice = "4E-07";
            var retrieveRequestsPrice = "5E-06";

            const int EndRange1 = 0;
            const decimal FinalCost = 0;

            _classUnderTest.Settings = RetrieveSettings(writeRequestsUsed, retrieveRequestsUsed, storageUsed, standardPricePerUnitTier1
                , standardPricePerUnitTier2, standardPricePerUnitTier3, writeRequestsPrice,
                retrieveRequestsPrice, EndRange1);
            _classUnderTest.StorageUsed = storageUsed;
            _classUnderTest.RetrieveRequestsUsed = retrieveRequestsUsed;
            _classUnderTest.WriteRequestsUsed = writeRequestsUsed;

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }
        
        [Fact]
        public void GetPrice_WhenAnyPricePerUnitIsNull_Return0()
        {
            // Arrange
            var writeRequestsUsed = 2000;
            var retrieveRequestsUsed = 3000;
            var storageUsed = 1024;
            var standardPricePerUnitTier1 = "0.023";
            
            var standardPricePerUnitTier3 = "0.022";
            var writeRequestsPrice = "4E-07";
            var retrieveRequestsPrice = "5E-06";
            var endRange1 = 0;

            const decimal FinalCost = 0;

            _classUnderTest.Settings = RetrieveSettings(writeRequestsUsed, retrieveRequestsUsed, storageUsed, standardPricePerUnitTier1
                , standardPricePerUnitTier2: null, standardPricePerUnitTier3, writeRequestsPrice,
                retrieveRequestsPrice, endRange1);
            _classUnderTest.StorageUsed = storageUsed;
            _classUnderTest.RetrieveRequestsUsed = retrieveRequestsUsed;
            _classUnderTest.WriteRequestsUsed = writeRequestsUsed;

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }

        #region RetrieveIntelligentTieringSettings
        private XElement RetrieveIntelligentTieringSettings(int writeRequestsUsed, int retrieveRequestsUsed, int storageUsed,
            int infrequentAccessMultiplier, string standardPricePerUnitTier1, string standardPricePerUnitTier2,
            string standardPricePerUnitTier3, string infrequentPricePerUnit, string writeRequestsPrice,
            string retrieveRequestsPrice)
        {
            var settingsXML =
                "{\"writeRequestsUsed\":\"" + writeRequestsUsed + "\"," +
                "\"retrieveRequestsUsed\":\"" + retrieveRequestsUsed + "\"," +
                "\"storageUsed\":\"" + storageUsed + "\"," +
                "\"infrequentAccessMultiplier\":\"" + infrequentAccessMultiplier + "\"," +
                "\"location\":\"[MOCK-LAND]\"," +
                "\"product0\":{" +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  first 50 TB month of storage used in Intelligent-Tiering, Frequent Access Tier\"," +
                "\"endRange\":\"51200\"," +
                "\"pricePerUnit\":\"" + standardPricePerUnitTier1 + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}," +
                "\"range1\":{" +
                "\"beginRange\":\"512000\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  over 500 TB month of storage used in Intelligent-Tiering, Frequent Access Tier\"," +
                "\"endRange\":\"-1\"," +
                "\"pricePerUnit\":\"" + standardPricePerUnitTier2 + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}," +
                "\"range2\":{" +
                "\"beginRange\":\"51200\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  next 450 TB month of storage used in Intelligent-Tiering, Frequent Access Tier\"," +
                "\"endRange\":\"512000\"," +
                "\"pricePerUnit\":\"" + standardPricePerUnitTier3 + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}}," +
                "\"purchaseOption\":[]," +
                "\"sku\":\"[MOCK] \"," +
                "\"term\":\"OnDemand\"}," +
                "\"product1\":{" +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK] GB-Month of storage used in Intelligent-Tiering, Infrequent Access Tier\"," +
                "\"endRange\":\"-1\"," +
                "\"pricePerUnit\":\"" + infrequentPricePerUnit + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}}," +
                "\"purchaseOption\":[]," +
                "\"sku\":\"[MOCK] \"," +
                "\"term\":\"OnDemand\"}," +
                "\"product2\":{" +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  per 1,000 PUT, COPY, POST, or LIST requests\"," +
                "\"endRange\":\"-1\"," +
                "\"pricePerUnit\":\"" + writeRequestsPrice + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"Requests\"}}," +
                "\"purchaseOption\":[]," +
                "\"sku\":\"[MOCK] \"," +
                "\"term\":\"OnDemand\"}," +
                "\"product3\":{\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  per 10,000 GET and all other requests\"," +
                "\"endRange\":\"-1\"," +
                "\"pricePerUnit\":\"" + retrieveRequestsPrice + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"Requests\"}}," +
                "\"purchaseOption\":[]," +
                "\"sku\":\"[MOCK] \"," +
                "\"term\":\"OnDemand\"}," +
                "\"productFamily\":\"Storage\"," +
                "\"storageClass\":\"Intelligent-Tiering\"," +
                "\"volumeType\":\"Intelligent-Tiering\"}";

            XNode node = JsonConvert.DeserializeXNode(settingsXML, "settings");
            var settings = XElement.Parse(node.ToString());
            return settings;
        }
        #endregion

        #region RetrieveSettings
        private XElement RetrieveSettings(int writeRequestsUsed, int retrieveRequestsUsed, int storageUsed,
            string? standardPricePerUnitTier1, string? standardPricePerUnitTier2, string? standardPricePerUnitTier3,
            string writeRequestsPrice, string retrieveRequestsPrice, int endRange1)
        {
            var settingsXML =
                "{\"productFamily\":\"Storage\"," +
                "\"volumeType\":\"Standard\"," +
                "\"storageClass\":\"General Purpose\"," +
                "\"location\":\"[MOCK - LAND]\"," +
                "\"writeRequestsUsed\":\"" + writeRequestsUsed + "\"," +
                "\"retrieveRequestsUsed\":\"" + retrieveRequestsUsed + "\"," +
                "\"storageUsed\":\"" + storageUsed + "\"," +
                "\"product0\":{" +
                "\"sku\":\"[MOCK]\"," +
                "\"term\":\"OnDemand\"," +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"purchaseOption\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"unit\":\"GB-Mo\"," +
                "\"endRange\":\""+ endRange1 + "\"," +
                "\"description\":\"[MOCK] per GB - first 50 TB month of storage used\"," +
                "\"rateCode\":\"[MOCK]\"," +
                "\"beginRange\":\"0\",\"currency\":\"USD\"," +
                "\"pricePerUnit\":\""+standardPricePerUnitTier1+"\"}" +
                ",\"range1\":{" +
                "\"unit\":\"GB-Mo\"," +
                "\"endRange\":\"-1\"," +
                "\"description\":\"[MOCK] per GB - storage used month over 500 TB\"," +
                "\"rateCode\":\"[MOCK]\"," +
                "\"beginRange\":\"512000\"," +
                "\"currency\":\"USD\"," +
                "\"pricePerUnit\":\"" + standardPricePerUnitTier2 + "\"}," +
                "\"range2\":{\"unit\":\"GB-Mo\"," +
                "\"endRange\":\"512000\"," +
                "\"description\":\"[MOCK] per GB - next 450 TB month of storage used\"," +
                "\"rateCode\":\"[MOCK]\"," +
                "\"beginRange\":\"51200\"," +
                "\"currency\":\"USD\"," +
                "\"pricePerUnit\":\"" + standardPricePerUnitTier3 + "\"}}}," +
                "\"product1\":{\"sku\":\"[MOCK]\"," +
                "\"term\":\"OnDemand\"," +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"purchaseOption\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"unit\":\"Requests\"," +
                "\"endRange\":\"-1\"," +
                "\"description\":\"[MOCK] per 1,000 PUT, COPY, POST, or LIST requests\"," +
                "\"rateCode\":\"[MOCK]\"," +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"pricePerUnit\":\"" + writeRequestsPrice + "\"}}}," +
                "\"product2\":{" +
                "\"sku\":\"[MOCK]\"," +
                "\"term\":\"OnDemand\"," +
                "\"leaseContractLength\":[]," +
                "\"offeringClass\":[]," +
                "\"purchaseOption\":[]," +
                "\"pricingDimension\":{" +
                "\"range0\":{" +
                "\"unit\":\"Requests\"," +
                "\"endRange\":\"-1\"," +
                "\"description\":\"[MOCK] per 10,000 GET and all other requests\"," +
                "\"rateCode\":\"[MOCK]\"," +
                "\"beginRange\":\"0\"," +
                "\"currency\":\"USD\"," +
                "\"pricePerUnit\":\"" + retrieveRequestsPrice + "\"}}}}";

            XNode node = JsonConvert.DeserializeXNode(settingsXML, "settings");
            var settings = XElement.Parse(node.ToString());
            return settings;
        }
        #endregion
    }
}