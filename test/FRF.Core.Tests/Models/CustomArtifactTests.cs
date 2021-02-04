using FRF.Core.Models;
using System.Xml.Linq;
using Xunit;

namespace FRF.Core.Tests.Models
{
    public class CustomArtifactTests
    {
        private readonly CustomArtifact _classUnderTest;

        public CustomArtifactTests()
        {
            _classUnderTest = new CustomArtifact();
        }

        [Fact]
        public void GetPrice_priceNodeExists_ReturnsPrice()
        {
            // Arrange
            var price = 70;
            _classUnderTest.Settings = XElement.Parse($"<settings><price>{price}</price></settings>");

            // Act
            var result = _classUnderTest.GetPrice();

            // Assert
            Assert.Equal(price, result);
        }

        [Fact]
        public void GetPrice_priceNodeNotExists_Returns0()
        {
            // Arrange
            _classUnderTest.Settings = XElement.Parse("<settings></settings>");

            // Act
            var result = _classUnderTest.GetPrice();

            // Assert
            Assert.Equal(0, result);
        }
    }
}
