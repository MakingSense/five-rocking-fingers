using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
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

        private ArtifactsRelation CreateArtifactsRelation(Artifact artifact1, Artifact artifact2)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new ArtifactsRelation()
            {
                Id = Guid.NewGuid(),
                Artifact1Id = artifact1.Id,
                Artifact1 = artifact1,
                Artifact2Id = artifact2.Id,
                Artifact2 = artifact2,
                Artifact1Property = "Mock 1 Property " + propertyId,
                Artifact2Property = "Mock 2 Property " + propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        private ArtifactsRelationInsertDTO CreateArtifactsRelationInsertDTO(int artifact1Id, int artifact2Id)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new ArtifactsRelationInsertDTO()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = "Mock 1 Property " + propertyId,
                Artifact2Property = "Mock 2 Property " + propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        private Provider CreateProvider()
        {
            var provider = new Provider
            {
                Name = "[Mock] Provider name"
            };

            return provider;
        }

        private ArtifactType CreateArtifactType(Provider provider)
        {
            var artifactType = new ArtifactType
            {
                Name = "[Mock] Artifact type name",
                Description = "[Mock] Artifact type description",
                Provider = provider
            };

            return artifactType;
        }

        private Project CreateProject(int id)
        {
            var project = new Project
            {
                Id = id,
                Name = "[MOCK] Project name",
                CreatedDate = DateTime.Now,
                ProjectCategories = new List<ProjectCategory>()
            };

            return project;
        }

        private Artifact CreateArtifact(int id, Project project, ArtifactType artifactType)
        {
            var artifact = new Artifact
            {
                Id = id,
                Name = "[Mock] Artifact name",
                CreatedDate = DateTime.Now,
                Project = project,
                ProjectId = project.Id,
                ArtifactType = artifactType,
                ArtifactTypeId = artifactType.Id,
                Settings = new XElement("Root",
                    new XElement("Child1", 1),
                    new XElement("Child2", 2),
                    new XElement("Child3", 3)
                )
            };

            return artifact;
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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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

            var artifacts = new ServiceResponse<List<Artifact>>(new List<Artifact> {artifact});

            _artifactsService
                .Setup(mock => mock.GetAll())
                .ReturnsAsync(artifacts);

            //Act
            var result = await _classUnderTest.GetAllAsync();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts.Value, returnValueList);

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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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

            var artifacts = new ServiceResponse<List<Artifact>>(new List<Artifact>{artifact});

            _artifactsService
                .Setup(mock => mock.GetAllByProjectId(It.IsAny<int>()))
                .ReturnsAsync(artifacts);

            //Act
            var result = await _classUnderTest.GetAllByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<ArtifactDTO>>(okResult.Value);
            var returnValueList = returnValue.ToList();

            AssertCompareList(artifacts.Value, returnValueList);

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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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
                .ReturnsAsync(new ServiceResponse<Artifact>(artifact));

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

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, "Error Message")));

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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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
                .ReturnsAsync(new ServiceResponse<Artifact>(artifact));

            //Act
            var result = await _classUnderTest.SaveAsync(artifactUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<ArtifactDTO>(okResult.Value);

            AssertCompareArtifactArtifactDTO(artifact, returnValue);
            _artifactsService.Verify(mock => mock.Save(It.Is<Artifact>(a => 
                a.Name == artifactUpsertDTO.Name
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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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
                .ReturnsAsync(new ServiceResponse<Artifact>(artifact));

            _artifactsService
                .Setup(mock => mock.Update(It.IsAny<Artifact>()))
                .ReturnsAsync(new ServiceResponse<Artifact>(artifact));

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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactUpdate = new ArtifactUpsertDTO
            {
                Name = "Artifact 1",
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
                .ReturnsAsync(new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, "Error Message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(artifactId, artifactUpdate);

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

            var provider = CreateProvider();

            var artifactTypeId = 1;
            var artifactType = new ArtifactType
            {
                Id = artifactTypeId,
                Name = "Artifact Type 1",
                Description = "Description of Artifact Type 1",
                Provider = provider
            };

            var artifactId = 1;
            var artifact = new Artifact
            {
                Id = artifactId,
                Name = "Artifact 1",
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
                .ReturnsAsync(new ServiceResponse<Artifact>(artifact));

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

            _artifactsService
                .Setup(mock => mock.Get(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Artifact>(new Error(ErrorCodes.ArtifactNotExists, "Error Message")));

            // Act
            var result = await _classUnderTest.DeleteAsync(artifactId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.Get(artifactId), Times.Once);
            _artifactsService.Verify(mock => mock.Delete(artifactId), Times.Never);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnOK()
        {
            // Arrange
            var artifactsRelationDtos = new List<ArtifactsRelationInsertDTO>();
            for (var i = 0; i < 3; i++)
            {
                var artifactRelation = CreateArtifactsRelationInsertDTO(i, ++i);
                artifactsRelationDtos.Add(artifactRelation);
            }
            _artifactsService
                .Setup(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(_mapper.Map<IList<ArtifactsRelation>>(artifactsRelationDtos)));
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
            var artifactsRelationDtos = new List<ArtifactsRelationInsertDTO>();
            var artifactRelation = CreateArtifactsRelationInsertDTO(1, 2);
            artifactsRelationDtos.Add(artifactRelation);

            _artifactsService
                .Setup(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid, "Error Message")));

            // Act
            var result = await _classUnderTest.SetRelationAsync(artifactsRelationDtos);

            // Assert
            var response = Assert.IsType<BadRequestResult>(result);
            Assert.IsNotType<List<ArtifactsRelationDTO>>(response);
            _artifactsService.Verify(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()), Times.Once);
        }
        [Fact]
        public async Task SetRelationAsync_ReturnBadRequest_WhenIsaBidirectionalRelation()
        {
            // Arrange
            var artifactsRelationDtos = new List<ArtifactsRelationInsertDTO>();
            artifactsRelationDtos.Add(new ArtifactsRelationInsertDTO()
            {
                Artifact1Id = 1,
                Artifact2Id = 2,
                Artifact1Property = "Mock 1 Property ",
                Artifact2Property = "Mock 2 Property ",
                RelationTypeId = 1
            });
            artifactsRelationDtos.Add(new ArtifactsRelationInsertDTO()
            {
                Artifact1Id = 2,
                Artifact2Id = 1,
                Artifact1Property = "Mock 1 Property ",
                Artifact2Property = "Mock 2 Property ",
                RelationTypeId = 0
            });

            _artifactsService
                .Setup(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid, "Error Message")));

            // Act
            var result = await _classUnderTest.SetRelationAsync(artifactsRelationDtos);

            // Assert
            var response = Assert.IsType<BadRequestResult>(result);
            Assert.IsNotType<List<ArtifactsRelationDTO>>(response);
            _artifactsService.Verify(mock => mock.SetRelationAsync(It.IsAny<List<ArtifactsRelation>>()), Times.Once);
        }

        [Fact]
        public async Task GetRelationsAsync_ReturnOk()
        {
            // Arrange
            var projectId = 1;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(1, project, artifactType);
            var artifact2 = CreateArtifact(2, project, artifactType);
            var artifactsRelationsList = new List<ArtifactsRelation>();
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            artifactsRelationsList.Add(artifactRelation);

            _artifactsService
                .Setup(mock => mock.GetAllRelationsOfAnArtifactAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(artifactsRelationsList));

            // Act
            var result = await _classUnderTest.GetRelationsAsync(artifact1.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IList<ArtifactsRelationDTO>>(okResult.Value);
            AssertCompareArtifactArtifactDTO(artifact1, returnValue[0].Artifact1);
            AssertCompareArtifactArtifactDTO(artifact2, returnValue[0].Artifact2);
            Assert.Equal(artifact1.Id, returnValue[0].Artifact1Id);
            Assert.Equal(artifact2.Id, returnValue[0].Artifact2Id);
            Assert.Equal(artifactRelation.Artifact1Property, returnValue[0].Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, returnValue[0].Artifact2Property);
            Assert.Equal(artifactRelation.RelationTypeId, returnValue[0].RelationTypeId);
            _artifactsService.Verify(mock => mock.GetAllRelationsOfAnArtifactAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetRelationsAsync_ReturnOkAndEmptyList()
        {
            // Arrange
            var project = CreateProject(1);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(1, project, artifactType);

            _artifactsService
                .Setup(mock => mock.GetAllRelationsOfAnArtifactAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new List<ArtifactsRelation>()));

            // Act
            var result = await _classUnderTest.GetRelationsAsync(artifact1.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IList<ArtifactsRelationDTO>>(okResult.Value);
            Assert.Empty(returnValue);
            _artifactsService.Verify(mock => mock.GetAllRelationsOfAnArtifactAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAllRelationsByProjectIdAsync_ReturnOk()
        {
            // Arrange
            var projectId = 1;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(1, project, artifactType);
            var artifact2 = CreateArtifact(2, project, artifactType);
            var artifactsRelationsList = new List<ArtifactsRelation>();
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            artifactsRelationsList.Add(artifactRelation);

            _artifactsService
                .Setup(mock => mock.GetAllRelationsByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(artifactsRelationsList));

            // Act
            var result = await _classUnderTest.GetAllRelationsByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IList<ArtifactsRelationDTO>>(okResult.Value);
            AssertCompareArtifactArtifactDTO(artifact1, returnValue[0].Artifact1);
            AssertCompareArtifactArtifactDTO(artifact2, returnValue[0].Artifact2);
            Assert.Equal(artifact1.Id, returnValue[0].Artifact1Id);
            Assert.Equal(artifact2.Id, returnValue[0].Artifact2Id);
            Assert.Equal(artifactRelation.Artifact1Property, returnValue[0].Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, returnValue[0].Artifact2Property);
            Assert.Equal(artifactRelation.RelationTypeId, returnValue[0].RelationTypeId);
            _artifactsService.Verify(mock => mock.GetAllRelationsByProjectIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetAllRelationsByProjectIdAsync_ReturnBadRequest()
        {
            // Arrange
            var projectId = 1;

            _artifactsService
                .Setup(mock => mock.GetAllRelationsByProjectIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(1, "[Mock] message")));

            // Act
            var result = await _classUnderTest.GetAllRelationsByProjectIdAsync(projectId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _artifactsService.Verify(mock => mock.GetAllRelationsByProjectIdAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRelationAsync_ReturnNoContent()
        {
            // Arrange
            var projectId = 1;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(1, project, artifactType);
            var artifact2 = CreateArtifact(2, project, artifactType);
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);

            _artifactsService
                .Setup(mock => mock.DeleteRelationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ServiceResponse<ArtifactsRelation>(artifactRelation));

            // Act
            var result = await _classUnderTest.DeleteRelationAsync(artifactRelation.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _artifactsService.Verify(mock => mock.DeleteRelationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRelationAsync_ReturnNotFound()
        {
            // Arrange
            var artifactRelationId = Guid.NewGuid();
           
            _artifactsService
                .Setup(mock => mock.DeleteRelationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ServiceResponse<ArtifactsRelation>(new Error(ErrorCodes.ArtifactNotExists, "[Mock] message")));

            // Act
            var result = await _classUnderTest.DeleteRelationAsync(artifactRelationId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.DeleteRelationAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRelationsAsync_ReturnNoContent()
        {
            // Arrange
            var projectId = 1;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(1, project, artifactType);
            var artifact2 = CreateArtifact(2, project, artifactType);
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            var artifactRelationList = new List<ArtifactsRelation>
            {
                artifactRelation
            };
            var artifactRelationIdsList = new List<Guid>
            {
                artifactRelation.Id
            };

            _artifactsService
                .Setup(mock => mock.DeleteRelationsAsync(It.IsAny<IList<Guid>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(artifactRelationList));

            // Act
            var result = await _classUnderTest.DeleteRelationsAsync(artifactRelationIdsList);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _artifactsService.Verify(mock => mock.DeleteRelationsAsync(It.IsAny<IList<Guid>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteRelationsAsync_ReturnNotFound()
        {
            // Arrange
            var artifactRelationIdsList = new List<Guid>
            {
                Guid.NewGuid()
            };

            _artifactsService
                .Setup(mock => mock.DeleteRelationsAsync(It.IsAny<IList<Guid>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, "[Mock] message")));

            // Act
            var result = await _classUnderTest.DeleteRelationsAsync(artifactRelationIdsList);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.DeleteRelationsAsync(It.IsAny<IList<Guid>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateArtifactsRelationsAsync_ReturnOk()
        {
            // Arrange
            var projectId = 1;
            var artifact1Id = 1;
            var artifact2Id = 2;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(artifact1Id, project, artifactType);
            var artifact2 = CreateArtifact(artifact2Id, project, artifactType);
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            var artifactsRelationsList = new List<ArtifactsRelation>
            {
                artifactRelation
            };
            var artifactRelationDto = new ArtifactsRelationUpdateDTO
            {
                Id = artifactRelation.Id,
                Artifact1Id = artifactRelation.Artifact1Id,
                Artifact2Id = artifactRelation.Artifact2Id,
                Artifact1Property = artifactRelation.Artifact1Property,
                Artifact2Property = artifactRelation.Artifact2Property,
                RelationTypeId = artifactRelation.RelationTypeId
            };
            var artifactsRelationsListDto = new List<ArtifactsRelationUpdateDTO>
            {
                artifactRelationDto
            };

            _artifactsService
                .Setup(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(artifactsRelationsList));

            // Act
            var result = await _classUnderTest.UpdateRelationsAsync(artifact1Id, artifactsRelationsListDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IList<ArtifactsRelationDTO>>(okResult.Value);
            AssertCompareArtifactArtifactDTO(artifact1, returnValue[0].Artifact1);
            AssertCompareArtifactArtifactDTO(artifact2, returnValue[0].Artifact2);
            Assert.Equal(artifact1.Id, returnValue[0].Artifact1Id);
            Assert.Equal(artifact2.Id, returnValue[0].Artifact2Id);
            Assert.Equal(artifactRelation.Artifact1Property, returnValue[0].Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, returnValue[0].Artifact2Property);
            Assert.Equal(artifactRelation.RelationTypeId, returnValue[0].RelationTypeId);
            _artifactsService.Verify(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateArtifactsRelationsAsync_ReturnNotFound()
        {
            // Arrange
            var projectId = 1;
            var artifact1Id = 1;
            var artifact2Id = 2;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(artifact1Id, project, artifactType);
            var artifact2 = CreateArtifact(artifact2Id, project, artifactType);
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            var artifactsRelationsListDto = new List<ArtifactsRelationUpdateDTO>
            {
                new ArtifactsRelationUpdateDTO
                {
                    Id = artifactRelation.Id,
                    Artifact1Id = artifactRelation.Artifact1Id,
                    Artifact2Id = artifactRelation.Artifact2Id,
                    Artifact1Property = artifactRelation.Artifact1Property,
                    Artifact2Property = artifactRelation.Artifact2Property,
                    RelationTypeId = artifactRelation.RelationTypeId
                }
            };

            _artifactsService
                .Setup(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.ArtifactNotExists, "[Mock] message")));

            // Act
            var result = await _classUnderTest.UpdateRelationsAsync(0, artifactsRelationsListDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _artifactsService.Verify(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateArtifactsRelationsAsync_ReturnBadRequest()
        {
            // Arrange
            var projectId = 1;
            var artifact1Id = 1;
            var artifact2Id = 2;
            var project = CreateProject(projectId);
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(artifact1Id, project, artifactType);
            var artifact2 = CreateArtifact(artifact2Id, project, artifactType);
            var artifactRelation = CreateArtifactsRelation(artifact1, artifact2);
            var artifactsRelationsListDto = new List<ArtifactsRelationUpdateDTO>
            {
                new ArtifactsRelationUpdateDTO
                {
                    Id = artifactRelation.Id,
                    Artifact1Id = artifactRelation.Artifact1Id,
                    Artifact2Id = artifactRelation.Artifact2Id,
                    Artifact1Property = artifactRelation.Artifact1Property,
                    Artifact2Property = artifactRelation.Artifact2Property,
                    RelationTypeId = artifactRelation.RelationTypeId
                },
                new ArtifactsRelationUpdateDTO
                {
                    Id = artifactRelation.Id,
                    Artifact1Id = artifactRelation.Artifact1Id,
                    Artifact2Id = artifactRelation.Artifact2Id,
                    Artifact1Property = artifactRelation.Artifact1Property,
                    Artifact2Property = artifactRelation.Artifact2Property,
                    RelationTypeId = artifactRelation.RelationTypeId
                }
            };

            _artifactsService
                .Setup(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()))
                .ReturnsAsync(new ServiceResponse<IList<ArtifactsRelation>>(new Error(ErrorCodes.RelationNotValid, "[Mock] message")));

            // Act
            var result = await _classUnderTest.UpdateRelationsAsync(artifact1Id, artifactsRelationsListDto);

            // Assert
            Assert.IsType<BadRequestResult>(result);
            _artifactsService.Verify(mock => mock.UpdateRelationAsync(It.IsAny<int>(), It.IsAny<IList<ArtifactsRelation>>()), Times.Once);
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
            Assert.Equal(artifact.ProjectId, artifactDTO.ProjectId);
            Assert.Equal(artifact.ArtifactType.Id, artifactDTO.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, artifactDTO.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, artifactDTO.ArtifactType.Description);
            Assert.Equal(artifact.ArtifactType.Provider.Id, artifactDTO.ArtifactType.Provider.Id);
            Assert.Equal(artifact.ArtifactType.Provider.Name, artifactDTO.ArtifactType.Provider.Name);
        }
    }
}
