using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ArtifactsControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<IArtifactsService> _artifactsService;

        private readonly ArtifactsController _classUnderTest;

        public ArtifactsControllerTests()
        {
            _artifactsService = new Mock<IArtifactsService>();
            _classUnderTest = new ArtifactsController(_artifactsService.Object, _mapper);
        }

        private ArtifactsRelationDTO CreateArtifactsRelationDTO(int artifact1Id, int artifact2Id)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new ArtifactsRelationDTO()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = "Mock 1 Property " + propertyId,
                Artifact2Property = "Mock 2 Property " + propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk()
        {
            //Arrange
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            List<Artifact> artifacts = new List<Artifact>
            {
                artifact
            };

            _artifactsService
                .Setup(mock => mock.GetAll())
                .ReturnsAsync(artifacts);

            //Act
            var result = await _classUnderTest.GetAllAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts, returnValueList);

            _artifactsService.Verify(mock => mock.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnsOk()
        {
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            List<Artifact> artifacts = new List<Artifact>
            {
                artifact
            };

            _artifactsService
                .Setup(mock => mock.GetAllByProjectId(It.IsAny<int>()))
                .ReturnsAsync(artifacts);

            //Act
            var result = await _classUnderTest.GetAllByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts, returnValueList);

            _artifactsService.Verify(mock => mock.GetAllByProjectId(projectId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsOk()
        {
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            var artifactTypeDTO = new ArtifactTypeDTO
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(artifact);

            //Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);

            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound()
        {
            // Arrange
            var artifactId = 1;

            //Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ReturnsOk()
        {
            // Arrange
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            var artifactUpsertDTO = new ArtifactUpsertDTO
            {
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                ProjectId = projectId,
                ArtifactTypeId = artifactTypeId
            };

            _artifactsService
                .Setup(mock => mock.Save(It.IsAny<Artifact>()))
                .ReturnsAsync(artifact);

            //Act
            var result = await _classUnderTest.SaveAsync(artifactUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);
            _artifactsService.Verify(mock => mock.Save(It.Is<Artifact>(a => 
                a.Name == artifactUpsertDTO.Name
                && a.Provider == artifactUpsertDTO.Provider
                && a.ProjectId == artifactUpsertDTO.ProjectId
                && a.ArtifactTypeId == artifactUpsertDTO.ArtifactTypeId)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOk()
        {
            // Arrange
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            var artifactUpsertDTO = new ArtifactUpsertDTO
            {
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                ProjectId = projectId,
                ArtifactTypeId = artifactTypeId
            };

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(artifact);

            _artifactsService
                .Setup(mock => mock.Update(It.IsAny<Artifact>()))
                .ReturnsAsync(artifact);

            //Act
            var result = await _classUnderTest.UpdateAsync(artifactId, artifactUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);

            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Update(artifact), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound()
        {
            // Arrange
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NoContent()
        {
            // Arrange
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Artifact 1",
                Owner = "Owner 1",
                Client = "Client 1",
                Budget = 50000,
                CreatedDate = DateTime.Now,
                ModifiedDate = null
            };

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                ProjectId = projectId,
                Project = project,
                ArtifactTypeId = artifactTypeId,
                ArtifactType = artifactType
            };

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(artifact);

            _artifactsService
                .Setup(mock => mock.Delete(It.IsAny<int>()));

            //Act
            var result = await _classUnderTest.DeleteAsync(artifactId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);

            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Delete(artifactId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound()
        {
            // Arrange
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Delete(artifactId), Times.Never);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnOK()
        {
            // Arrange
            var artifactsRelationDtos = new List<ArtifactsRelationDTO>();
            for (var i = 0; i < 3; i++)
            {
                var artifactRelation = CreateArtifactsRelationDTO(i, ++i);
                artifactsRelationDtos.Add(artifactRelation);
            }
            _artifactsService
                .Setup(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()))
                .ReturnsAsync(_mapper.Map<IList<ArtifactsRelation>>(artifactsRelationDtos));
            // Act
            var result = await _classUnderTest.SetRelationAsync(artifactsRelationDtos);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ArtifactsRelationDTO>>(okResult.Value);
            _artifactsService.Verify(mock=>mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()),Times.Once);

        }

        [Fact]
        public async Task SetRelationAsync_ReturnBadRequest()
        {
            // Arrange
            var artifactsRelationDtos = new List<ArtifactsRelationDTO>();
            var artifactRelation = CreateArtifactsRelationDTO(1, 2);
            artifactsRelationDtos.Add(artifactRelation);

            // Act
            var result = await _classUnderTest.SetRelationAsync(artifactsRelationDtos);

            // Assert
            var response = Assert.IsType<BadRequestResult>(result);
            Assert.IsNotType<List<ArtifactsRelationDTO>>(response);
            _artifactsService.Verify(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()), Times.Once);
        }

        internal void AssertCompareList(List<Artifact> artifacts, List<ArtifactDTO> returnValueList)
        {
            Assert.Equal(artifacts.Count, returnValueList.Count);

            for (int i = 0; i < artifacts.Count; i++)
            {
                AssertCompareArtifactArtifactDTO(artifacts[i], returnValueList[i]);
            }
        }

        internal void AssertCompareArtifactArtifactDTO(Artifact artifact, ArtifactDTO artifactDTO)
        {
            Assert.Equal(artifact.Id, artifactDTO.Id);
            Assert.Equal(artifact.Name, artifactDTO.Name);
            Assert.Equal(artifact.Provider, artifactDTO.Provider);
            Assert.True(XNode.DeepEquals(artifact.Settings, artifactDTO.Settings));
            Assert.Equal(artifact.ProjectId, artifactDTO.ProjectId);
            Assert.Equal(artifact.ArtifactType.Id, artifactDTO.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, artifactDTO.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, artifactDTO.ArtifactType.Description);
        }
    }
}
