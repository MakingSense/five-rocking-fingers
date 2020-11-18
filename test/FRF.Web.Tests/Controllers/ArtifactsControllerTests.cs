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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ArtifactsControllerTests
    {
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IArtifactsService> _artifactsService;

        private readonly ArtifactsController _classUnderTest;

        public ArtifactsControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _artifactsService = new Mock<IArtifactsService>();
            _classUnderTest = new ArtifactsController(_artifactsService.Object, _mapper.Object);
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

            var artifactTypeDTO = new ArtifactTypeDTO
            {
                Id = artifactType.Id,
                Name = artifactType.Name,
                Description = artifactType.Description
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

            var artifactDTO = new ArtifactDTO
            {
                Id = artifact.Id,
                Name = artifact.Name,
                Provider = artifact.Provider,
                Settings = artifact.Settings,
                ProjectId = projectId,
                ArtifactType = artifactTypeDTO
            };

            List<Artifact> artifacts = new List<Artifact>
            {
                artifact
            };

            List<ArtifactDTO> artifactsDTO = new List<ArtifactDTO>
            {
                artifactDTO
            };

            _artifactsService
                .Setup(mock => mock.GetAll())
                .ReturnsAsync(artifacts);

            _mapper
                .Setup(mock => mock.Map<IEnumerable<ArtifactDTO>>(It.IsAny<List<Artifact>>()))
                .Returns(artifactsDTO);

            //Act
            var result = await _classUnderTest.GetAllAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts, returnValueList);
            
            _artifactsService.Verify(mock => mock.GetAll(), Times.Once);
            _mapper.Verify(mock => mock.Map<IEnumerable<ArtifactDTO>>(It.Is<List<Artifact>>(a => a.Contains(artifact))), Times.Once);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync()
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

            var artifactTypeDTO = new ArtifactTypeDTO
            {
                Id = artifactType.Id,
                Name = artifactType.Name,
                Description = artifactType.Description
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

            var artifactDTO = new ArtifactDTO
            {
                Id = artifact.Id,
                Name = artifact.Name,
                Provider = artifact.Provider,
                Settings = artifact.Settings,
                ProjectId = projectId,
                ArtifactType = artifactTypeDTO
            };

            List<Artifact> artifacts = new List<Artifact>
            {
                artifact
            };

            List<ArtifactDTO> artifactsDTO = new List<ArtifactDTO>
            {
                artifactDTO
            };

            _artifactsService
                .Setup(mock => mock.GetAllByProjectId(It.IsAny<int>()))
                .ReturnsAsync(artifacts);

            _mapper
                .Setup(mock => mock.Map<IEnumerable<ArtifactDTO>>(It.IsAny<List<Artifact>>()))
                .Returns(artifactsDTO);

            //Act
            var result = await _classUnderTest.GetAllByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts, returnValueList);

            _artifactsService.Verify(mock => mock.GetAllByProjectId(projectId), Times.Once);
            _mapper.Verify(mock => mock.Map<IEnumerable<ArtifactDTO>>(It.Is<List<Artifact>>(a => a.Contains(artifact))), Times.Once);
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

            var artifactDTO = new ArtifactDTO
            {
                Id = 1,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                ProjectId = projectId,
                ArtifactType = artifactTypeDTO
            };

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(artifact);

            _mapper
                .Setup(mock => mock.Map<ArtifactDTO>(It.IsAny<Artifact>()))
                .Returns(artifactDTO);

            //Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);

            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _mapper.Verify(mock => mock.Map<ArtifactDTO>(It.Is<Artifact>(a => a.Equals(artifact))), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound()
        {
            // Arrange
            var artifactId = 1;

            //Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _mapper.Verify(mock => mock.Map<ArtifactDTO>(It.IsAny<Artifact>()), Times.Never);
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
        
            var artifactTypeDTO = new ArtifactTypeDTO
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactDTO = new ArtifactDTO
            {
                Id = 1,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                ProjectId = projectId,
                ArtifactType = artifactTypeDTO
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

            _mapper
                .Setup(mock => mock.Map<Artifact>(It.IsAny<ArtifactUpsertDTO>()))
                .Returns(artifact);

            _mapper
                .Setup(mock => mock.Map<ArtifactDTO>(It.IsAny<Artifact>()))
                .Returns(artifactDTO);

            //Act
            var result = await _classUnderTest.SaveAsync(artifactUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, artifactDTO);
            _artifactsService.Verify(mock => mock.Save(artifact), Times.Once);
            _mapper.Verify(mock => mock.Map<ArtifactDTO>(It.Is<Artifact>(a => a.Equals(artifact))), Times.Once);
            _mapper.Verify(mock => mock.Map<Artifact>(It.Is<ArtifactUpsertDTO>(a => a.Equals(artifactUpsertDTO))), Times.Once);
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

            var artifactTypeDTO = new ArtifactTypeDTO
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1"
            };

            var artifactDTO = new ArtifactDTO
            {
                Id = 1,
                Name = "Artifact 1",
                Provider = "AWS",
                Settings = new XElement("Root",
                                new XElement("Child1", 1),
                                new XElement("Child2", 2),
                                new XElement("Child3", 3)
                            ),
                ProjectId = projectId,
                ArtifactType = artifactTypeDTO
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

            _mapper
                .Setup(mock => mock.Map<ArtifactDTO>(It.IsAny<Artifact>()))
                .Returns(artifactDTO);

            _mapper
                .Setup(mock => mock.Map(It.IsAny<ArtifactUpsertDTO>(), It.IsAny<Artifact>()))
                .Returns(artifact);

            //Act
            var result = await _classUnderTest.UpdateAsync(artifactId, artifactUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);

            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Update(artifact), Times.Once);
            _mapper.Verify(mock => mock.Map(It.Is<ArtifactUpsertDTO>(a => a.Equals(artifactUpsertDTO)), It.Is<Artifact>(a => a.Equals(artifact))), Times.Once);
            _mapper.Verify(mock => mock.Map<ArtifactDTO>(It.Is<Artifact>(a => a.Equals(artifact))), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound()
        {
            // Arrange
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.GetAsync(artifactId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _mapper.Verify(mock => mock.Map(It.IsAny<ArtifactUpsertDTO>(), It.IsAny<Artifact>()), Times.Never);
            _mapper.Verify(mock => mock.Map<ArtifactDTO>(It.IsAny<Artifact>()), Times.Never);
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
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Delete(artifactId), Times.Never);
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
