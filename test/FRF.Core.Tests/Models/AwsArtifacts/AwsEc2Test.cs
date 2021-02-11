using FRF.Core.Models.AwsArtifacts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace FRF.Core.Tests.Models.AwsArtifacts
{
    public class AwsEc2Test
    {
        private readonly AwsEc2 _classUnderTest;

        public AwsEc2Test()
        {
            _classUnderTest = new AwsEc2();
        }

        [Fact]
        public void GetPrice_Gp2()
        {
            // Arrange

            const decimal FinalCost = 1231.5354194444444444444444444m;

            _classUnderTest.Settings = CreateArtifactSettingsGp2();

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }

        [Fact]        
        public void GetPrice_Gp3()
        {
            // Arrange

            const decimal FinalCost = 1241.6154194444444444444444444m;

            _classUnderTest.Settings = CreateArtifactSettingsGp3();

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }

        [Fact]
        public void GetPrice_Io1()
        {
            // Arrange

            const decimal FinalCost = 1520.0754194444444444444444444m;

            _classUnderTest.Settings = CreateArtifactSettingsIo1();

            // Act
            var result = _classUnderTest.GetPrice();
            // Assert
            Assert.IsType<decimal>(result);
            Assert.Equal(FinalCost, result);
        }

        #region CreateArtifactSettingsGp2
        private XElement CreateArtifactSettingsGp2()
        {
            XElement settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),                        
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("pricingDimensions", 
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("volumeApiName", "gp2"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.1200000000)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransferIntraRegion", 1024),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                );

            return settings;
        }
        #endregion
        #region CreateArtifactSettingsGp3
        private XElement CreateArtifactSettingsGp3()
        {
            XElement settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("volumeApiName", "gp3"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.096)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product3",
                        new XElement("numberOfIopsPerMonth", 4000),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.006)
                            )
                        )
                    ),
                    new XElement("product4",
                        new XElement("numberOfMbpsThroughput", 225),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 49.1520000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransferIntraRegion", 1024),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                );

            return settings;
        }
        #endregion
        #region CreateArtifactSettingsIo1
        private XElement CreateArtifactSettingsIo1()
        {
            XElement settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("volumeApiName", "io1"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.138)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product3",
                        new XElement("numberOfIopsPerMonth", 4000),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0720000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransferIntraRegion", 1024),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                );

            return settings;
        }
        #endregion        
    }
}
