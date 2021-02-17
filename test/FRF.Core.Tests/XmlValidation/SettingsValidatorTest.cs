using FRF.Core.Models;
using FRF.Core.XmlValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace FRF.Core.Tests.XmlValidation
{
    public class SettingsValidatorTest
    {
        private readonly SettingsValidator _classUnderTest;

        public SettingsValidatorTest()
        {
            _classUnderTest = new SettingsValidator();
        }

        private Project CreateProject()
        {
            var project = new Project();
            project.Id = 1;
            project.Name = "[MOCK] Project name";
            project.CreatedDate = DateTime.Now;
            project.ProjectCategories = new List<ProjectCategory>();

            return project;
        }

        private ArtifactType CreateArtifactType(string name)
        {
            var artifactType = new ArtifactType();
            artifactType.Id = 1;
            artifactType.Name = name;
            artifactType.Description = "[Mock] Artifact type description";

            return artifactType;
        }

        private Provider CreateProvider(string name)
        {
            var provider = new Provider()
            {
                Id = 1,
                Name = name
            };

            return provider;
        }

        [Fact]
        public void ValidateSettings_CustomArtifactRetunsTrue()
        {
            // Arange
            var artifactType = CreateArtifactType(ArtifactTypes.Custom);
            artifactType.Provider = CreateProvider(ArtifactTypes.Custom);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings", new XElement("price", 100))
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSettings_CustomArtifactRetunsFalse()
        {
            // Arange
            var artifactType = CreateArtifactType(ArtifactTypes.Custom);
            artifactType.Provider = CreateProvider(ArtifactTypes.Custom);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings")
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateSettings_AwsEc2Gp2_RetunsTrue()
        {
            // Arange
            var artifactType = CreateArtifactType(AwsEc2Descriptions.ServiceValue);
            artifactType.Provider = CreateProvider(ArtifactTypes.Aws);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Compute Instance"),
                        new XElement("term", "Reserved"),
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("offeringClass", "Convertible"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("operatingSystem", "Windows"),
                        new XElement("preInstalledSw", "SQL Ent"),
                        new XElement("instanceType", "t3a.xlarge"),
                        new XElement("vcpu", "4"),
                        new XElement("memory", "16 GiB"),
                        new XElement("networkPerformance", "Up to 5 Gigabit"),
                        new XElement("storage", "EBS only"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Hrs"),
                                new XElement("endRange", -1),
                                new XElement("description", "Windows with SQL Server Enterprise (Amazon VPC), t3a.xlarge reserved instance applied"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("unit", "Quantity"),
                                new XElement("endRange", -1),
                                new XElement("description", "Upfront Fee"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.2TG2D8R56U"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Storage"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp2"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Gb-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.12 per GB-month of General Purpose SSD (gp2) provisioned storage - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.1200000000)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Storage Snapshot"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp2"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.055 per GB-Month of snapshot data stored - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransfer", 1024),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Data Transfer"),
                        new XElement("term", "OnDemand"),
                        new XElement("transferType", "IntraRegion"),
                        new XElement("fromLocationType", "AWS Region"),
                        new XElement("fromLocation", "US West (N. California)"),
                        new XElement("toLocationType", "AWS Region"),
                        new XElement("toLocation", "US West (N. California)"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.010 per GB Regional Data Transfer - in/out/between AZs or using public IP or Elastic IP addresses"),
                                new XElement("rateCode", "FDJZG2WS6XZNB2B3.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                )
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSettings_AwsEc2Gp3_RetunsTrue()
        {
            // Arange
            var artifactType = CreateArtifactType(AwsEc2Descriptions.ServiceValue);
            artifactType.Provider = CreateProvider(ArtifactTypes.Aws);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Compute Instance"),
                        new XElement("term", "Reserved"),
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("offeringClass", "Convertible"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("operatingSystem", "Windows"),
                        new XElement("preInstalledSw", "SQL Ent"),
                        new XElement("instanceType", "t3a.xlarge"),
                        new XElement("vcpu", "4"),
                        new XElement("memory", "16 GiB"),
                        new XElement("networkPerformance", "Up to 5 Gigabit"),
                        new XElement("storage", "EBS only"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Hrs"),
                                new XElement("endRange", -1),
                                new XElement("description", "Windows with SQL Server Enterprise (Amazon VPC), t3a.xlarge reserved instance applied"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("unit", "Quantity"),
                                new XElement("endRange", -1),
                                new XElement("description", "Upfront Fee"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.2TG2D8R56U"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Storage"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp3"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Gb-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.12 per GB-month of General Purpose SSD (gp2) provisioned storage - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.096)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Storage Snapshot"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp3"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.055 per GB-Month of snapshot data stored - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product3",
                        new XElement("numberOfIopsPerMonth", 4000),
                        new XElement("sku", "AW6X9CDG3FS9XTDS"),
                        new XElement("productFamily", "System Operation"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp3"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "IOPS-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.072 per IOPS-month provisioned (io2) - US West (Northern California)"),
                                new XElement("rateCode", "AW6X9CDG3FS9XTDS.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.006)
                            )
                        )
                    ),
                    new XElement("product4",
                        new XElement("numberOfMbpsThroughput", 225),
                        new XElement("sku", "AW6X9CDG3FS9XTDS"),
                        new XElement("productFamily", "Provisioned Throughput"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "gp3"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GiBps-mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.048 per provisioned MiBps-month of gp3 - US West (N. California)"),
                                new XElement("rateCode", "ZDC2V7MBY9Z73F7V.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 49.1520000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransfer", 1024),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Data Transfer"),
                        new XElement("term", "OnDemand"),
                        new XElement("transferType", "IntraRegion"),
                        new XElement("fromLocationType", "AWS Region"),
                        new XElement("fromLocation", "US West (N. California)"),
                        new XElement("toLocationType", "AWS Region"),
                        new XElement("toLocation", "US West (N. California)"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.010 per GB Regional Data Transfer - in/out/between AZs or using public IP or Elastic IP addresses"),
                                new XElement("rateCode", "FDJZG2WS6XZNB2B3.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                )
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSettings_AwsEc2Io1_RetunsTrue()
        {
            // Arange
            var artifactType = CreateArtifactType(AwsEc2Descriptions.ServiceValue);
            artifactType.Provider = CreateProvider(ArtifactTypes.Aws);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings",
                    new XElement("product0",
                        new XElement("hoursUsedPerMonth", 730),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Compute Instance"),
                        new XElement("term", "Reserved"),
                        new XElement("leaseContractLength", "3 yr"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("offeringClass", "Convertible"),
                        new XElement("purchaseOption", "Partial Upfront"),
                        new XElement("operatingSystem", "Windows"),
                        new XElement("preInstalledSw", "SQL Ent"),
                        new XElement("instanceType", "t3a.xlarge"),
                        new XElement("vcpu", "4"),
                        new XElement("memory", "16 GiB"),
                        new XElement("networkPerformance", "Up to 5 Gigabit"),
                        new XElement("storage", "EBS only"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Hrs"),
                                new XElement("endRange", -1),
                                new XElement("description", "Windows with SQL Server Enterprise (Amazon VPC), t3a.xlarge reserved instance applied"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.8225000000)
                            ),
                            new XElement("range1",
                                new XElement("unit", "Quantity"),
                                new XElement("endRange", -1),
                                new XElement("description", "Upfront Fee"),
                                new XElement("rateCode", "WY6M7B237ABTJB8K.38NPMPTW36.2TG2D8R56U"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 21616)
                            )
                        )
                    ),
                    new XElement("product1",
                        new XElement("numberOfGbStorageInEbs", 30),
                        new XElement("sku", "WY6M7B237ABTJB8K"),
                        new XElement("productFamily", "Storage"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "io1"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "Gb-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.12 per GB-month of General Purpose SSD (gp2) provisioned storage - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.138)
                            )
                        )
                    ),
                    new XElement("product2",
                        new XElement("numberOfSnapshotsPerMonth", 59.83),
                        new XElement("numberOfGbChangedPerSnapshot", 3),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Storage Snapshot"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "io1"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.055 per GB-Month of snapshot data stored - US West (Northern California)"),
                                new XElement("rateCode", "UZ6TZ5NDAH7AGJPJ.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0550000000)
                            )
                        )
                    ),
                    new XElement("product3",
                        new XElement("numberOfIopsPerMonth", 4000),
                        new XElement("sku", "AW6X9CDG3FS9XTDS"),
                        new XElement("productFamily", "System Operation"),
                        new XElement("term", "OnDemand"),
                        new XElement("volumeApiName", "io1"),
                        new XElement("location", "US West (N. California)"),
                        new XElement("locationType", "AWS Region"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "IOPS-Mo"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.072 per IOPS-month provisioned (io2) - US West (Northern California)"),
                                new XElement("rateCode", "AW6X9CDG3FS9XTDS.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0720000000)
                            )
                        )
                    ),
                    new XElement("product5",
                        new XElement("numberOfGbTransfer", 1024),
                        new XElement("sku", "3F2BXQPS4TRZ6SR6"),
                        new XElement("productFamily", "Data Transfer"),
                        new XElement("term", "OnDemand"),
                        new XElement("transferType", "IntraRegion"),
                        new XElement("fromLocationType", "AWS Region"),
                        new XElement("fromLocation", "US West (N. California)"),
                        new XElement("toLocationType", "AWS Region"),
                        new XElement("toLocation", "US West (N. California)"),
                        new XElement("pricingDimensions",
                            new XElement("range0",
                                new XElement("unit", "GB"),
                                new XElement("endRange", -1),
                                new XElement("description", "$0.010 per GB Regional Data Transfer - in/out/between AZs or using public IP or Elastic IP addresses"),
                                new XElement("rateCode", "FDJZG2WS6XZNB2B3.JRTCKXETXF.6YS6EN2CT7"),
                                new XElement("beginRange", 0),
                                new XElement("currency", "USD"),
                                new XElement("pricePerUnit", 0.0100000000)
                            )
                        )
                    )
                )
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateSettings_AwsEc2RetunsFalse()
        {
            // Arange
            var artifactType = CreateArtifactType(AwsEc2Descriptions.ServiceValue);
            artifactType.Provider = CreateProvider(ArtifactTypes.Aws);
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = project,
                ArtifactTypeId = artifactType.Id,
                ArtifactType = artifactType,
                Settings = new XElement("settings")
            };

            // Act
            var result = _classUnderTest.ValidateSettings(artifact);

            // Assert
            Assert.False(result);
        }
    }
}
