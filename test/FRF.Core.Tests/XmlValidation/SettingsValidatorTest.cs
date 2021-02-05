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

        private ArtifactType CreateArtifactType()
        {
            var artifactType = new ArtifactType();
            artifactType.Id = 1;
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
            var artifactType = CreateArtifactType();
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
            var artifactType = CreateArtifactType();
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
    }
}
