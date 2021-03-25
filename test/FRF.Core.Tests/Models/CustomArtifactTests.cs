using FRF.Core.Models;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace FRF.Core.Tests.Models
{
    public class CustomArtifactTests
    {
        [Fact]
        public void GetPrice_priceNodeExists_ReturnsPrice()
        {
            // Arrange
            var price = 70;
            var settings = XElement.Parse($"<settings><price>{price}</price></settings>");
            var _classUnderTest = new CustomArtifact(settings);

            // Act
            var result = _classUnderTest.GetPrice();

            // Assert
            Assert.Equal(price, result);
        }

        [Fact]
        public void GetPrice_priceNodeNotExists_Returns0()
        {
            // Arrange
            var settings = XElement.Parse("<settings></settings>");
            var _classUnderTest = new CustomArtifact(settings);

            // Act
            var result = _classUnderTest.GetPrice();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void RelationalFieldsGeneratedCorrectly()
        {
            // Arrange
            XElement settings = new XElement("settings",
                    new XElement("settingName1", 100, new XAttribute("type", SettingTypes.Decimal)),
                    new XElement("settingName2", 200, new XAttribute("type", SettingTypes.NaturalNumber)),
                    new XElement("settingName3", 300, new XAttribute("type", SettingTypes.Decimal)));

            // Act
            var _classUnderTest = new CustomArtifact(settings);

            // Assert
            var settingsList = settings.Elements().ToList();

            Assert.Equal(settingsList.Count, _classUnderTest.RelationalFields.Count);
            foreach(var setting in settingsList)
            {
                Assert.Equal(setting.Attribute("type").Value.ToString(), _classUnderTest.RelationalFields[setting.Name.ToString()]);
            }            
        }
    }
}
