using AutoMapper;
using FRF.Core.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using ArtifactsRelation = FRF.Core.Models.ArtifactsRelation;
using CoreModels = FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.XmlValidation;
using System.Xml.Linq;

namespace FRF.Core.Tests.Services
{
    public class ArtifactsServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ArtifactsService _classUnderTest;
        private readonly DbContextOptions<DataAccessContextForTest> ContextOptions;
        private readonly Mock<ISettingsValidator> _settingsValidator;

        public ArtifactsServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _settingsValidator = new Mock<ISettingsValidator>();

            _classUnderTest = new ArtifactsService(_dataAccess, _mapper, _settingsValidator.Object);
        }

        private Provider CreateProvider()
        {
            var provider = new Provider();
            provider.Name = "[Mock] Provider name";
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            return provider;
        }

        private ArtifactType CreateArtifactType(Provider provider)
        {
            var artifactType = new ArtifactType();
            artifactType.Name = "[Mock] Artifact type name";
            artifactType.Description = "[Mock] Artifact type description";
            artifactType.Provider = provider;
            _dataAccess.ArtifactType.Add(artifactType);
            _dataAccess.SaveChanges();

            return artifactType;
        }

        private Project CreateProject()
        {
            var project = new Project();
            project.Name = "[MOCK] Project name";
            project.CreatedDate = DateTime.Now;
            project.ProjectCategories = new List<ProjectCategory>();
            _dataAccess.Projects.Add(project);
            _dataAccess.SaveChanges();

            return project;
        }

        private Artifact CreateArtifact(Project project, ArtifactType artifactType)
        {   
            var random = new Random();
            var artifact = new Artifact()
            {
                Name = "[Mock] Artifact name "+ random.Next(500),
                CreatedDate = DateTime.Now,
                Project = project,
                ProjectId = project.Id,
                ArtifactType = artifactType,
                ArtifactTypeId = artifactType.Id
            };
            _dataAccess.Artifacts.Add(artifact);
            _dataAccess.SaveChanges();

            return artifact;
        }

        private CoreModels.ArtifactsRelation CreateArtifactsRelationModel(int artifact1Id,int artifact2Id)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = "Mock 1 Property "+propertyId,
                Artifact2Property = "Mock 2 Property "+propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.Artifact>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifact.ArtifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifact.ArtifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.Artifact>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnList()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.Artifact>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifact.ArtifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifact.ArtifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnExceptionNoProject()
        {
            // Arange
            var projectId = 1;

            // Act
            var result = await _classUnderTest.GetAllByProjectId(projectId);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.Artifact>>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnEmptyList()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<CoreModels.Artifact>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsArtifact()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Get(artifact.Id);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<CoreModels.Artifact>(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifact.ArtifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifact.ArtifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arange
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.Get(artifactId);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ArtifactNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsArtifact()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactToSave = new CoreModels.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;
            artifactToSave.Settings = new XElement("Settings");

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<CoreModels.Artifact>(result.Value);

            Assert.Equal(artifactToSave.Name, resultValue.Name);
            Assert.Null(resultValue.ModifiedDate);
            Assert.Equal(artifactToSave.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifactToSave.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(project.Id, resultValue.Project.Id);
            Assert.Equal(project.Name, resultValue.Project.Name);

            Assert.Equal(artifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionInvalidArtifactSettings()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactToSave = new CoreModels.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;
            artifactToSave.Settings = new XElement("Settings");

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidArtifactSettings, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoProjectWithId()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifactToSave = new CoreModels.Artifact()
            {
                Name = "[Mock] Artifact name 1",
                CreatedDate = DateTime.Now,
                ProjectId = 999,
                Project = new CoreModels.Project(),
                ArtifactTypeId = artifactType.Id,
                ArtifactType = new CoreModels.ArtifactType()
                {
                    Id = artifactType.Id,
                    Description = artifactType.Description,
                    Name = artifactType.Name
                }
            };

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            var project = CreateProject();
            var artifactToSave = new CoreModels.Artifact() 
            { 
                Name = "[Mock] Artifact name 1",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = new CoreModels.Project {
                    Id = project.Id,
                    Name = project.Name,
                    CreatedDate = project.CreatedDate,
                    ProjectCategories = new List<CoreModels.ProjectCategory>()
                },
                ArtifactTypeId = 999,
                ArtifactType = new CoreModels.ArtifactType()
            };

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ArtifactTypeNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsArtifact()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var newProject = new Project()
            {
                Name = "[Mock] Project after update",
                CreatedDate = DateTime.Now,
                ProjectCategories = new List<ProjectCategory>()
            };
            _dataAccess.Projects.Add(newProject);
            _dataAccess.SaveChanges();

            var newArtifactType = new ArtifactType()
            {
                Name = "[Mock] Artifact type name",
                Description = "[Mock] Artifact type description",
                Provider = provider
            };
            _dataAccess.ArtifactType.Add(newArtifactType);
            _dataAccess.SaveChanges();

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
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

            Assert.Equal(newArtifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(newArtifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionInvalidArtifactSettings()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var newProject = new Project()
            {
                Name = "[Mock] Project after update",
                CreatedDate = DateTime.Now,
                ProjectCategories = new List<ProjectCategory>()
            };
            _dataAccess.Projects.Add(newProject);
            _dataAccess.SaveChanges();

            var newArtifactType = new ArtifactType()
            {
                Name = "[Mock] Artifact type name",
                Description = "[Mock] Artifact type description",
                Provider = provider
            };
            _dataAccess.ArtifactType.Add(newArtifactType);
            _dataAccess.SaveChanges();

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = newProject.Id,
                Project = _mapper.Map<CoreModels.Project>(newProject),
                ArtifactTypeId = newArtifactType.Id,
                ArtifactType = _mapper.Map<CoreModels.ArtifactType>(newArtifactType),
                Settings = new XElement("Settings")
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidArtifactSettings, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactWithId()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                ArtifactTypeId = artifactType.Id
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ArtifactNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoProjectWithId()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = 999,
                ArtifactTypeId = artifactType.Id
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                ArtifactTypeId = 999
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ArtifactTypeNotExists, result.Error.Code);
        }

        [Fact]
        public async Task Delete_ReturnsArtifact()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Delete(artifact.Id);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<CoreModels.Artifact>(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task Delete_ReturnsNull()
        {
            // Arange
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Delete(0);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.ArtifactNotExists, result.Error.Code);
        }
        
        [Fact]
        public async Task SetRelationAsync_ReturnsListOfRelations()
        {
            // Arange
            var artifactsRelationToSave = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var project = CreateProject();
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id);
                artifactsRelationToSave.Add(artifactRelation);

                var artifactRelationDb =
                    _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(
                        CreateArtifactsRelationModel(artifact1Id, artifact2Id));
                await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationDb);
                artifactsRelationInDb.Add(artifactRelationDb);
                var art = CreateArtifact(project, artifactType);
                i++;
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifactsRelationToSave);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response.Value);
            for (var j = 0; j < resultValue.Count; j++)
            {
                Assert.Equal(artifactsRelationToSave[j].Artifact1Id, resultValue[j].Artifact1Id);
                Assert.Equal(artifactsRelationToSave[j].Artifact2Id, resultValue[j].Artifact2Id);
                Assert.Equal(artifactsRelationToSave[j].Artifact1Property, resultValue[j].Artifact1Property);
                Assert.Equal(artifactsRelationToSave[j].Artifact2Property, resultValue[j].Artifact2Property);
            }
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyArtifactExcept()
        {
            // Arange
            var artifactsRelationToSave = new List<ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var artifactRelation = CreateArtifactsRelationModel(i, ++i);
                artifactsRelationToSave.Add(artifactRelation);
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotValid, response.Error.Code);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyArtifactRepeated()
        {
            // Arange
            var artifactsRelationToSave = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var project = CreateProject();
                var artifact1 = CreateArtifact(project, artifactType);
                var artifact2 = CreateArtifact(project, artifactType);
                var artifact1Id = artifact1.Id;
                var artifact2Id = artifact2.Id;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id);
                artifactsRelationToSave.Add(artifactRelation);

                var artifactRelationDb = _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(artifactRelation);
                await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationDb);
                artifactsRelationInDb.Add(artifactRelationDb);
                i++;
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationAlreadyExisted, response.Error.Code);
            Assert.Null(response.Value);
            Assert.Equal(artifactsRelationToSave[0].Artifact1Property, artifactsRelationInDb[0].Artifact1Property);
            Assert.Equal(artifactsRelationToSave[0].Artifact2Property, artifactsRelationInDb[0].Artifact2Property);
            Assert.Equal(artifactsRelationToSave[0].Artifact2Id, artifactsRelationInDb[0].Artifact2Id);
            Assert.Equal(artifactsRelationToSave[0].Artifact1Id, artifactsRelationInDb[0].Artifact1Id);
        }
        
        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyBidirectionalRelationSameRelationType()
        {
            // Arange
            var artifactsRelationToSave = new List<ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);

            var artifactRelation1 = new ArtifactsRelation()
            {
                Artifact1Id = artifact1.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "Mock 1 Property",
                Artifact2Property = "Mock 2 Property",
                RelationTypeId = 1
            };
            var artifactRelation2 = new ArtifactsRelation()
            {
                Artifact1Id = artifact2.Id,
                Artifact2Id = artifact1.Id,
                Artifact1Property = "Mock 2 Property",
                Artifact2Property = "Mock 1 Property",
                RelationTypeId = 1
            };
            artifactsRelationToSave.Add(artifactRelation2);
            await _dataAccess.ArtifactsRelation.AddAsync(
                _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(artifactRelation1));

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationAlreadyExisted, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyBidirectionalRelationDiferentRelationType()
        {
            // Arange
            var artifactsRelationToSave = new List<ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);

            var artifactRelation1 = new ArtifactsRelation()
            {
                Artifact1Id = artifact1.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "Mock 1 Property",
                Artifact2Property = "Mock 2 Property",
                RelationTypeId = 1
            };
            var artifactRelation2 = new ArtifactsRelation()
            {
                Artifact1Id = artifact1.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "Mock 1 Property",
                Artifact2Property = "Mock 2 Property",
                RelationTypeId = 0
            };
            artifactsRelationToSave.Add(artifactRelation2);
            await _dataAccess.ArtifactsRelation.AddAsync(
                _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(artifactRelation1));
            
            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationAlreadyExisted, response.Error.Code);
            Assert.Null(response.Value);
        }
        [Fact]
        public async Task UpdateRelationAsync_ReturnsNull_WhenIsAnyArtifactExcept()
        {
            // Arange
            var artifactsRelationUpdated = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var project = CreateProject();
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id);
                artifactsRelationUpdated.Add(artifactRelation);

                var artifactRelationDb = _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(artifactRelation);
                await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationDb);
                artifactsRelationInDb.Add(artifactRelationDb);
                i++;
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifactsRelationUpdated[0].Artifact1Id, artifactsRelationUpdated);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(response.Error.Code, ErrorCodes.ArtifactNotExists);
            Assert.Null(response.Value);
            Assert.Equal(artifactsRelationUpdated[0].Artifact1Property, artifactsRelationInDb[0].Artifact1Property);
            Assert.Equal(artifactsRelationUpdated[0].Artifact2Property, artifactsRelationInDb[0].Artifact2Property);
            Assert.Equal(artifactsRelationUpdated[0].Artifact2Id, artifactsRelationInDb[0].Artifact2Id);
            Assert.Equal(artifactsRelationUpdated[0].Artifact1Id, artifactsRelationInDb[0].Artifact1Id);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsListOfRelations()
        {
            // Arange
            var artifactsRelationUpdated = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var artifactBase = CreateArtifact(project, artifactType);
            var artifactIdToUpdate = artifactBase.Id;
            var i = 0;
            while (i < 3)
            {
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifactIdToUpdate;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id);


                var artifactRelationDb = _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(CreateArtifactsRelationModel(artifact1Id, artifact2Id));
                await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationDb);

                artifactRelation.Id = artifactRelationDb.Id;
                artifactsRelationUpdated.Add(artifactRelation);
                artifactsRelationInDb.Add(artifactRelationDb);

                i++;
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifactIdToUpdate, artifactsRelationUpdated);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response.Value);
            for (var j = 0; j < resultValue.Count; j++)
            {
                Assert.Equal(artifactsRelationUpdated[j].Artifact1Id, resultValue[j].Artifact1Id);
                Assert.Equal(artifactsRelationUpdated[j].Artifact2Id, resultValue[j].Artifact2Id);
                Assert.Equal(artifactsRelationUpdated[j].Artifact1Property, resultValue[j].Artifact1Property);
                Assert.Equal(artifactsRelationUpdated[j].Artifact2Property, resultValue[j].Artifact2Property);
            }
        }

        [Fact]
        public async Task GetAllRelationsByProjectIdAsync_ReturnsListOfRelations()
        {
            // Arange
            var project = CreateProject();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelationDb =
                    _mapper.Map<DataAccess.EntityModels.ArtifactsRelation>(
                        CreateArtifactsRelationModel(artifact1Id, artifact2Id));
                await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationDb);
                artifactsRelationInDb.Add(artifactRelationDb);
                i++;
            }

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.GetAllRelationsByProjectIdAsync(project.Id);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response.Value);
            for (var j = 0; j < resultValue.Count; j++)
            {
                Assert.Equal(artifactsRelationInDb[j].Artifact1.ProjectId, resultValue[j].Artifact1.ProjectId);
                Assert.Equal(artifactsRelationInDb[j].Artifact2.ProjectId, resultValue[j].Artifact2.ProjectId);
            }
        }

        [Fact]
        public async Task GetAllRelationsByProjectIdAsync_ReturnsNull_WhenProjectIdNotExist()
        {
            // Arange
            var project = CreateProject();
            var projectIdToSearch = ++project.Id;

            // Act
            var response = await _classUnderTest.GetAllRelationsByProjectIdAsync(projectIdToSearch);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.ProjectNotExists, response.Error.Code);
        }

        [Fact]
        public async Task GetAllRelationsOfAnArtifactAsync_ReturnsListOfRelations()
        {
            // Arange
            var project = CreateProject();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);
            var artifactRelation = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id);
            var artifactRelationMapped = _mapper.Map<FRF.DataAccess.EntityModels.ArtifactsRelation>(artifactRelation);

            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationMapped);
            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.GetAllRelationsOfAnArtifactAsync(artifact1.Id);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response.Value);
            Assert.Equal(artifactRelationMapped.Id, resultValue[0].Id);
            Assert.Equal(artifactRelation.Artifact1Id, resultValue[0].Artifact1Id);
            Assert.Equal(artifactRelation.Artifact2Id, resultValue[0].Artifact2Id);
            Assert.Equal(artifactRelation.RelationTypeId, resultValue[0].RelationTypeId);
            Assert.Equal(artifactRelation.Artifact1Property, resultValue[0].Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, resultValue[0].Artifact2Property);
        }

        [Fact]
        public async Task GetAllRelationsOfAnArtifactAsync_ReturnsArtifactNotExists()
        {
            // Arange
            var artifactId = 0;

            // Act
            var response = await _classUnderTest.GetAllRelationsOfAnArtifactAsync(artifactId);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.ArtifactNotExists, response.Error.Code);
        }

        [Fact]
        public async Task DeleteRelationAsync_ReturnsNull_RelationNotExist()
        {
            // Arange
            var artifactRelationId = new Guid();

            // Act
            var response = await _classUnderTest.DeleteRelationAsync(artifactRelationId);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<ArtifactsRelation>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotExists, response.Error.Code);
        }

        [Fact]
        public async Task DeleteRelationAsync_ReturnsRelation()
        {
            // Arange
            var project = CreateProject();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);
            var artifactRelation = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id);
            var artifactRelationMapped = _mapper.Map<FRF.DataAccess.EntityModels.ArtifactsRelation>(artifactRelation);

            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationMapped);
            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.DeleteRelationAsync(artifactRelationMapped.Id);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<ArtifactsRelation>>(response);
            var resultValue = Assert.IsAssignableFrom<ArtifactsRelation>(response.Value);
            Assert.Equal(artifactRelationMapped.Id, resultValue.Id);
            Assert.Equal(artifactRelation.Artifact1Id, resultValue.Artifact1Id);
            Assert.Equal(artifactRelation.Artifact2Id, resultValue.Artifact2Id);
            Assert.Equal(artifactRelation.RelationTypeId, resultValue.RelationTypeId);
            Assert.Equal(artifactRelation.Artifact1Property, resultValue.Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, resultValue.Artifact2Property);
        }
    }
}
