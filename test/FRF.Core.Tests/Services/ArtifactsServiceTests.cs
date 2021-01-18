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
using FRF.Core.Response;

namespace FRF.Core.Tests.Services
{
    public class ArtifactsServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ArtifactsService _classUnderTest;
        private readonly DbContextOptions<DataAccessContextForTest> ContextOptions;

        public ArtifactsServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new ArtifactsService(_dataAccess, _mapper);
        }

        private ArtifactType CreateArtifactType()
        {
            var artifactType = new ArtifactType();
            artifactType.Name = "[Mock] Artifact type name";
            artifactType.Description = "[Mock] Artifact type description";
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
            var artifact = new Artifact()
            {
                Name = "[Mock] Artifact name",
                Provider = "[Mock] AWS",
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

        private Models.ArtifactsRelation CreateArtifactsRelationModel(int artifact1Id,int artifact2Id)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new Models.ArtifactsRelation()
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
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Artifact>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.Provider, resultValue.Provider);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Artifact>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnList()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Artifact>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.Provider, resultValue.Provider);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnExceptionNoProject()
        {
            // Arange
            var projectId = 1;

            // Act
            var result = await _classUnderTest.GetAllByProjectId(projectId);

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Artifact>>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnEmptyList()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Artifact>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAsync_ReturnsArtifact()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Get(artifact.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<Models.Artifact>(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.Provider, resultValue.Provider);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arange
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.Get(artifactId);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ArtifactNotExists);
        }

        [Fact]
        public async Task SaveAsync_ReturnsArtifact()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();

            var artifactToSave = new Models.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.Provider = "[Mock] AWS";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<Models.Artifact>(result.Value);

            Assert.Equal(artifactToSave.Name, resultValue.Name);
            Assert.Equal(artifactToSave.Provider, resultValue.Provider);
            Assert.Null(resultValue.ModifiedDate);
            Assert.Equal(artifactToSave.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifactToSave.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(project.Id, resultValue.Project.Id);
            Assert.Equal(project.Name, resultValue.Project.Name);

            Assert.Equal(artifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifactType.Description, resultValue.ArtifactType.Description);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoProjectWithId()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var artifactToSave = new Models.Artifact()
            {
                Name = "[Mock] Artifact name 1",
                Provider = "[Mock] AWS",
                CreatedDate = DateTime.Now,
                ProjectId = 999,
                Project = new Models.Project(),
                ArtifactTypeId = artifactType.Id,
                ArtifactType = new Models.ArtifactType()
                {
                    Id = artifactType.Id,
                    Description = artifactType.Description,
                    Name = artifactType.Name
                }
            };

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            var project = CreateProject();
            var artifactToSave = new Models.Artifact() 
            { 
                Name = "[Mock] Artifact name 1",
                Provider = "[Mock] AWS",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = new Models.Project {
                    Id = project.Id,
                    Name = project.Name,
                    CreatedDate = project.CreatedDate,
                    ProjectCategories = new List<Models.ProjectCategory>()
                },
                ArtifactTypeId = 999,
                ArtifactType = new Models.ArtifactType()
            };

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ArtifactTypeNotExists);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsArtifact()
        {
            // Arange
            var artifactType = CreateArtifactType();
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
                Description = "[Mock] Artifact type description"
            };
            _dataAccess.ArtifactType.Add(newArtifactType);
            _dataAccess.SaveChanges();

            var artifactToUpdate = new Models.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                Provider = "[Mock] Updated provider",
                CreatedDate = DateTime.Now,
                ProjectId = newProject.Id,
                Project = _mapper.Map<Models.Project>(newProject),
                ArtifactTypeId = newArtifactType.Id,
                ArtifactType = _mapper.Map<Models.ArtifactType>(newArtifactType)
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<Models.Artifact>(result.Value);

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

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactWithId()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifactToUpdate = new Models.Artifact()
            {
                Id = 1,
                Name = "[Mock] Updated name",
                Provider = "[Mock] Updated provider",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                ArtifactTypeId = artifactType.Id
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ArtifactNotExists);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoProjectWithId()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var artifactToUpdate = new Models.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                Provider = "[Mock] Updated provider",
                CreatedDate = DateTime.Now,
                ProjectId = 999,
                ArtifactTypeId = artifactType.Id
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var artifactToUpdate = new Models.Artifact()
            {
                Id = artifact.Id,
                Name = "[Mock] Updated name",
                Provider = "[Mock] Updated provider",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                ArtifactTypeId = 999
            };

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ArtifactTypeNotExists);
        }

        [Fact]
        public async Task Delete_ReturnsArtifact()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Delete(artifact.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<Models.Artifact>(result.Value);

            Assert.Equal(artifact.Id, resultValue.Id);
            Assert.Equal(artifact.Name, resultValue.Name);
            Assert.Equal(artifact.Provider, resultValue.Provider);
            Assert.Equal(artifact.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(artifact.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, resultValue.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, resultValue.Project.Id);
            Assert.Equal(artifact.Project.Name, resultValue.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, resultValue.ArtifactType.Description);
        }

        [Fact]
        public async Task Delete_ReturnsNull()
        {
            // Arange
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            var result = await _classUnderTest.Delete(0);

            // Assert
            Assert.IsType<ServiceResponse<Models.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error.Code, ErrorCodes.ArtifactNotExists);
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
                var artifactType = CreateArtifactType();
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
            Assert.IsType<ServiceResponse<IList<Models.ArtifactsRelation>>>(response);
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
            Assert.IsType<ServiceResponse<IList<Models.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(response.Error.Code, ErrorCodes.RelationNotValid);
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
                var artifactType = CreateArtifactType();
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
            Assert.IsType<ServiceResponse<IList<Models.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(response.Error.Code, ErrorCodes.RelationAlreadyExisted);

            Assert.Equal(artifactsRelationInDb[0].Artifact1Property, artifactsRelationToSave[0].Artifact1Property);
            Assert.Equal(artifactsRelationInDb[0].Artifact2Property, artifactsRelationToSave[0].Artifact2Property);
            Assert.Equal(artifactsRelationInDb[0].Artifact2Id, artifactsRelationToSave[0].Artifact2Id);
            Assert.Equal(artifactsRelationInDb[0].Artifact1Id, artifactsRelationToSave[0].Artifact1Id);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsNull_WhenIsAnyArtifactExcept()
        {
            // Arange
            var artifactsRelationUpdated = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            var artifactId = 0;
            while (i < 3)
            {
                var artifactType = CreateArtifactType();
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
            var response = await _classUnderTest.UpdateRelationAsync(artifactsRelationUpdated[0].Artifact1Id,artifactsRelationUpdated);

            // Assert
            Assert.Null(response);
            Assert.Equal(artifactsRelationInDb[0].Artifact1Property, artifactsRelationUpdated[0].Artifact1Property);
            Assert.Equal(artifactsRelationInDb[0].Artifact2Property, artifactsRelationUpdated[0].Artifact2Property);
            Assert.Equal(artifactsRelationInDb[0].Artifact2Id, artifactsRelationUpdated[0].Artifact2Id);
            Assert.Equal(artifactsRelationInDb[0].Artifact1Id, artifactsRelationUpdated[0].Artifact1Id);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsListOfRelations()
        {
            // Arange
            var artifactsRelationUpdated = new List<ArtifactsRelation>();
            var artifactsRelationInDb = new List<DataAccess.EntityModels.ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var artifactType = CreateArtifactType();
                var project = CreateProject();
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id);
                artifactsRelationUpdated.Add(artifactRelation);

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
            var response = await _classUnderTest.UpdateRelationAsync(artifactsRelationUpdated[0].Artifact1Id,artifactsRelationUpdated);

            // Assert
            var result = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response);
            for (var j = 0; j < result.Count; j++)
            {
                Assert.Equal(artifactsRelationUpdated[j].Artifact1Id, result[j].Artifact1Id);
                Assert.Equal(artifactsRelationUpdated[j].Artifact2Id, result[j].Artifact2Id);
                Assert.Equal(artifactsRelationUpdated[j].Artifact1Property, result[j].Artifact1Property);
                Assert.Equal(artifactsRelationUpdated[j].Artifact2Property, result[j].Artifact2Property);
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
                var artifactType = CreateArtifactType();
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
            var result = Assert.IsAssignableFrom<IList<ArtifactsRelation>>(response);
            for (var j = 0; j < result.Count; j++)
            {
                Assert.Equal(artifactsRelationInDb[j].Artifact1.ProjectId, result[j].Artifact1.ProjectId);
                Assert.Equal(artifactsRelationInDb[j].Artifact2.ProjectId, result[j].Artifact2.ProjectId);
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
            Assert.Null(response);
        }
    }
}