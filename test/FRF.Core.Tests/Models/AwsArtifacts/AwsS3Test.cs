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

            _classUnderTest.Settings = RetrieveSettings(writeRequestsUsed, retrieveRequestsUsed, storageUsed,
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

        private XElement RetrieveSettings(int writeRequestsUsed, int retrieveRequestsUsed, int storageUsed, int infrequentAccessMultiplier, 
            string standardPricePerUnitTier1,
            string standardPricePerUnitTier2,
            string standardPricePerUnitTier3,string infrequentPricePerUnit, string writeRequestsPrice, string retrieveRequestsPrice)
        {
            var settingsXML =
                "{\"writeRequestsUsed\":\""+writeRequestsUsed+"\"," +
                "\"retrieveRequestsUsed\":\""+retrieveRequestsUsed+"\"," +
                "\"storageUsed\":\""+storageUsed+ "\"," +
                "\"infrequentAccessMultiplier\":\""+ infrequentAccessMultiplier + "\"," +
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
                "\"pricePerUnit\":\""+ standardPricePerUnitTier1 + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}," +
                "\"range1\":{" +
                "\"beginRange\":\"512000\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  over 500 TB month of storage used in Intelligent-Tiering, Frequent Access Tier\"," +
                "\"endRange\":\"-1\"," +
                "\"pricePerUnit\":\""+ standardPricePerUnitTier2 + "\"," +
                "\"rateCode\":\"[MOCK] \"," +
                "\"unit\":\"GB-Mo\"}," +
                "\"range2\":{" +
                "\"beginRange\":\"51200\"," +
                "\"currency\":\"USD\"," +
                "\"description\":\"[MOCK]  next 450 TB month of storage used in Intelligent-Tiering, Frequent Access Tier\"," +
                "\"endRange\":\"512000\"," +
                "\"pricePerUnit\":\""+standardPricePerUnitTier3+"\"," +
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
                "\"pricePerUnit\":\""+infrequentPricePerUnit+"\"," +
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
                "\"pricePerUnit\":\""+ retrieveRequestsPrice+"\"," +
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
    }
}