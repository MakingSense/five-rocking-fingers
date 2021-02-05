using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.XmlValidation
{
    public class SettingsValidatorTest
    {
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

        [Fact]
        public async Task ValidateSettings_CustomArtifactRetunsTrue()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();

            var artifact = new Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                Provider = "[Mock] Updated provider",
                CreatedDate = DateTime.Now,
                ProjectId = newProject.Id,
                Project = _mapper.Map<CoreModels.Project>(newProject),
                ArtifactTypeId = newArtifactType.Id,
                ArtifactType = _mapper.Map<CoreModels.ArtifactType>(newArtifactType),
                Settings = new XElement("Settings")
            };



            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<CoreModels.Artifact>(result.Value);

            Assert.Equal(artifactToUpdate.Id, resultValue.Id);
            Assert.Equal(artifactToUpdate.Name, resultValue.Name);
            Assert.Equal(artifactToUpdate.Provider, resultValue.Provider);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.NotEqual(artifactToUpdate.CreatedDate, resultValue.CreatedDate); // This because it shouldn't allow CreatedDate change
            Assert.NotNull(resultValue.ModifiedDate);
            Assert.Equal(artifactToUpdate.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifactToUpdate.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(newProject.Id, resultValue.Project.Id);
            Assert.Equal(newProject.Name, resultValue.Project.Name);

            Assert.Equal(newArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(newArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(newArtifactType.Description, resultValue.ArtifactType.Description);
        }
    }
}
