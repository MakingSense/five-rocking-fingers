using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class BudgetServiceTests
    {
        private readonly BudgetService _classUnderTest;
        private readonly Mock<IArtifactsService> _artifactsService;

        public BudgetServiceTests()
        {
            _artifactsService = new Mock<IArtifactsService>();
            _classUnderTest = new BudgetService(_artifactsService.Object);
        }

        private ArtifactType CreateArtifactType()
        {
            var artifactType = new ArtifactType();
            artifactType.Name = "[Mock] Artifact type name";
            artifactType.Description = "[Mock] Artifact type description";

            return artifactType;
        }

        private Project CreateProject()
        {
            var project = new Project();
            project.Name = "[MOCK] Project name";
            project.CreatedDate = DateTime.Now;
            project.ProjectCategories = new List<ProjectCategory>();
            return project;
        }

        private Artifact CreateArtifact(Project project, ArtifactType artifactType)
        {
            var random = new Random();
            var artifact = new Artifact()
            {
                Name = "[Mock] Artifact name " + random.Next(500),
                Provider = "[Mock] AWS",
                CreatedDate = DateTime.Now,
                Project = project,
                ProjectId = project.Id,
                ArtifactType = artifactType,
                ArtifactTypeId = artifactType.Id
            };

            return artifact;
        }

        [Fact]
        public async Task GetBudget_ReturnsBudget()
        {
            //Arrange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var artifactsList = new List<Artifact>
            {
                artifact
            };

            _artifactsService.Setup(mock => mock.GetAllByProjectId(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<Artifact>>(artifactsList));

            //Act
            var response = await _classUnderTest.GetBudget(project.Id);

            //Assert
            Assert.IsType<ServiceResponse<decimal>>(response);
            Assert.True(response.Success);
            Assert.Equal(response.Value, artifact.GetPrice());
            _artifactsService.Verify(mock => mock.GetAllByProjectId(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetBudget_ReturnsError()
        {
            //Arrange
            var projectId = 0;
            _artifactsService.Setup(mock => mock.GetAllByProjectId(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<List<Artifact>>(new Error(ErrorCodes.ProjectNotExists, "[Mock] Message")));

            //Act
            var response = await _classUnderTest.GetBudget(projectId);

            //Assert
            Assert.IsType<ServiceResponse<decimal>>(response);
            Assert.False(response.Success);
            Assert.Equal(response.Error.Code, ErrorCodes.ProjectNotExists);
            _artifactsService.Verify(mock => mock.GetAllByProjectId(It.IsAny<int>()), Times.Once);
        }
    }
}
