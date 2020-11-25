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

namespace FRF.Core.Tests.Services
{
    public class ArtifactsServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IMapper> _mapper;
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ArtifactsService _classUnderTest;
        protected DbContextOptions<DataAccessContextForTest> ContextOptions { get; }
        public ArtifactsServiceTests()
        {
            _configuration = new Mock<IConfiguration>();
            _mapper = new Mock<IMapper>();

            ContextOptions = new DbContextOptionsBuilder<DataAccessContextForTest>()
                    .UseInMemoryDatabase(databaseName: "Test")
                    .Options;
            _dataAccess = new DataAccessContextForTest(ContextOptions, _configuration.Object);

            _classUnderTest = new ArtifactsService(_configuration.Object, _dataAccess, _mapper.Object);

            _mapper
                .Setup(mock => mock.Map<Models.Project>(It.IsAny<Project>()))
                .Returns<Project>((input) => new Models.Project()
                {
                    Id = input.Id,
                    Name = input.Name
                });

            _mapper
                .Setup(mock => mock.Map<Models.ArtifactType>(It.IsAny<ArtifactType>()))
                .Returns<ArtifactType>((input) => new Models.ArtifactType()
                {
                    Id = input.Id,
                    Name = input.Name, 
                    Description = input.Description
                });
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

        // GetAll
        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var mapperReturn = new List<Models.Artifact>();
            mapperReturn.Add(new Models.Artifact { 
                Id = artifact.Id,
                Name = artifact.Name,
                Provider = artifact.Provider,
                CreatedDate = artifact.CreatedDate,
                ProjectId = artifact.ProjectId,
                Project = _mapper.Object.Map<Models.Project>(artifact.Project),
                ArtifactTypeId = artifact.ArtifactTypeId,
                ArtifactType = _mapper.Object.Map<Models.ArtifactType>(artifact.ArtifactType)
            });
            _mapper
                .Setup(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()))
                .Returns(mapperReturn);

            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            var okResult = Assert.IsType<List<Models.Artifact>>(result);

            Assert.Single(result);

            Assert.Equal(artifact.Id, result[0].Id);
            Assert.Equal(artifact.Name, result[0].Name);
            Assert.Equal(artifact.Provider, result[0].Provider);
            Assert.Equal(artifact.CreatedDate, result[0].CreatedDate);
            Assert.Equal(artifact.ProjectId, result[0].ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, result[0].ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, result[0].Project.Id);
            Assert.Equal(artifact.Project.Name, result[0].Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, result[0].ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, result[0].ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, result[0].ArtifactType.Description);

            _mapper.Verify(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _mapper
                .Setup(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()))
                .Returns(new List<Models.Artifact>());

            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            Assert.IsType<List<Models.Artifact>>(result);
            Assert.Empty(result);
            _mapper.Verify(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()), Times.Once);
        }

        // GetAllByProjectId
        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnList()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            var mapperReturn = new List<Models.Artifact>();
            mapperReturn.Add(new Models.Artifact
            {
                Id = artifact.Id,
                Name = artifact.Name,
                Provider = artifact.Provider,
                CreatedDate = artifact.CreatedDate,
                ProjectId = artifact.ProjectId,
                Project = _mapper.Object.Map<Models.Project>(artifact.Project),
                ArtifactTypeId = artifact.ArtifactTypeId,
                ArtifactType = _mapper.Object.Map<Models.ArtifactType>(artifact.ArtifactType)
            });
            _mapper
                .Setup(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()))
                .Returns(mapperReturn);

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            var okResult = Assert.IsType<List<Models.Artifact>>(result);

            Assert.Single(result);

            Assert.Equal(artifact.Id, result[0].Id);
            Assert.Equal(artifact.Name, result[0].Name);
            Assert.Equal(artifact.Provider, result[0].Provider);
            Assert.Equal(artifact.CreatedDate, result[0].CreatedDate);
            Assert.Equal(artifact.ProjectId, result[0].ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, result[0].ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, result[0].Project.Id);
            Assert.Equal(artifact.Project.Name, result[0].Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, result[0].ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, result[0].ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, result[0].ArtifactType.Description);

            _mapper.Verify(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnExceptionNoProject()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.GetAllByProjectId(1));
            _mapper.Verify(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()), Times.Never);
        }

        [Fact]
        public async Task GetAllByProjectIdAsync_ReturnEmptyList()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            var artifactType = CreateArtifactType();
            var project = CreateProject();

            _mapper
                .Setup(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()))
                .Returns(new List<Models.Artifact>());

            // Act
            var result = await _classUnderTest.GetAllByProjectId(project.Id);

            // Assert
            Assert.IsType<List<Models.Artifact>>(result);
            Assert.Empty(result);
            _mapper.Verify(mock => mock.Map<List<Models.Artifact>>(It.IsAny<IEnumerable<Artifact>>()), Times.Once);
        }

        // Get
        [Fact]
        public async Task GetAsync_ReturnsArtifact()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            _mapper
                .Setup(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()))
                .Returns(new Models.Artifact
                {
                    Id = artifact.Id,
                    Name = artifact.Name,
                    Provider = artifact.Provider,
                    CreatedDate = artifact.CreatedDate,
                    ProjectId = artifact.ProjectId,
                    Project = _mapper.Object.Map<Models.Project>(artifact.Project),
                    ArtifactTypeId = artifact.ArtifactTypeId,
                    ArtifactType = _mapper.Object.Map<Models.ArtifactType>(artifact.ArtifactType)
                });

            // Act
            var result = await _classUnderTest.Get(artifact.Id);

            // Assert
            var okResult = Assert.IsType<Models.Artifact>(result);

            Assert.Equal(artifact.Id, result.Id);
            Assert.Equal(artifact.Name, result.Name);
            Assert.Equal(artifact.Provider, result.Provider);
            Assert.Equal(artifact.CreatedDate, result.CreatedDate);
            Assert.Equal(artifact.ProjectId, result.ProjectId);
            Assert.Equal(artifact.ArtifactTypeId, result.ArtifactTypeId);

            Assert.Equal(artifact.Project.Id, result.Project.Id);
            Assert.Equal(artifact.Project.Name, result.Project.Name);

            Assert.Equal(artifact.ArtifactType.Id, result.ArtifactType.Id);
            Assert.Equal(artifact.ArtifactType.Name, result.ArtifactType.Name);
            Assert.Equal(artifact.ArtifactType.Description, result.ArtifactType.Description);

            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();
            
            var artifactId = 1;

            // Act
            var result = await _classUnderTest.Get(artifactId);

            // Assert
            Assert.Null(result);
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.Is<Artifact>(a => a.Id == artifactId)), Times.Never);
        }

        // Save
        [Fact]
        public async Task SaveAsync_ReturnsArtifact()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            var artifactType = CreateArtifactType();
            var project = CreateProject();

            var artifactToSave = new Models.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.Provider = "[Mock] AWS";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;

            _mapper
                .Setup(mock => mock.Map<Artifact>(It.IsAny<Models.Artifact>()))
                .Returns(new Artifact
                {
                    Name = artifactToSave.Name,
                    Provider = artifactToSave.Provider,
                    ProjectId = artifactToSave.ProjectId,
                    ArtifactTypeId = artifactToSave.ArtifactTypeId,
                });

            _mapper
                .Setup(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()))
                .Returns<Artifact>((input) => new Models.Artifact
                {
                    Id = input.Id,
                    Name = input.Name,
                    Provider = input.Provider,
                    CreatedDate = input.CreatedDate,
                    ProjectId = input.ProjectId,
                    Project = _mapper.Object.Map<Models.Project>(input.Project),
                    ArtifactTypeId = input.ArtifactTypeId,
                    ArtifactType = _mapper.Object.Map<Models.ArtifactType>(input.ArtifactType)
                });

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.Equal(artifactToSave.Name, result.Name);
            Assert.Equal(artifactToSave.Provider, result.Provider);
            Assert.Null(result.ModifiedDate);
            Assert.Equal(artifactToSave.ProjectId, result.ProjectId);
            Assert.Equal(artifactToSave.ArtifactTypeId, result.ArtifactTypeId);

            Assert.Equal(project.Id, result.Project.Id);
            Assert.Equal(project.Name, result.Project.Name);

            Assert.Equal(artifactType.Id, result.ArtifactType.Id);
            Assert.Equal(artifactType.Name, result.ArtifactType.Name);
            Assert.Equal(artifactType.Description, result.ArtifactType.Description);

            _mapper.Verify(mock => mock.Map<Artifact>(It.IsAny<Models.Artifact>()), Times.Once);
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Once);
            _mapper.Verify(mock => mock.Map<Models.Project>(It.IsAny<Project>()), Times.Once);
            _mapper.Verify(mock => mock.Map<Models.ArtifactType>(It.IsAny<ArtifactType>()), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoProjectWithId()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.Save(artifactToSave));
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Never);
        }

        [Fact]
        public async Task SaveAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.Save(artifactToSave));
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Never);
        }

        // Update
        [Fact]
        public async Task UpdateAsync_ReturnsArtifact()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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
                Project = _mapper.Object.Map<Models.Project>(newProject),
                ArtifactTypeId = newArtifactType.Id,
                ArtifactType = _mapper.Object.Map<Models.ArtifactType>(newArtifactType)
            };

            _mapper
                .Setup(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()))
                .Returns<Artifact>((input) => new Models.Artifact()
                {
                    Id = input.Id,
                    Name = input.Name,
                    Provider = input.Provider,
                    CreatedDate = input.CreatedDate,
                    ModifiedDate = (DateTime)input.ModifiedDate,
                    ProjectId = input.ProjectId,
                    Project = _mapper.Object.Map<Models.Project>(input.Project),
                    ArtifactTypeId = input.ArtifactTypeId,
                    ArtifactType = _mapper.Object.Map<Models.ArtifactType>(input.ArtifactType),
                });

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<Models.Artifact>(result);

            Assert.Equal(artifactToUpdate.Id, result.Id);
            Assert.Equal(artifactToUpdate.Name, result.Name);
            Assert.Equal(artifactToUpdate.Provider, result.Provider);
            Assert.Equal(artifact.CreatedDate, result.CreatedDate);
            Assert.NotEqual(artifactToUpdate.CreatedDate, result.CreatedDate); // This because it shouldn't allow CreatedDate change
            Assert.NotNull(result.ModifiedDate);
            Assert.Equal(artifactToUpdate.ProjectId, result.ProjectId);
            Assert.Equal(artifactToUpdate.ArtifactTypeId, result.ArtifactTypeId);

            Assert.Equal(newProject.Id, result.Project.Id);
            Assert.Equal(newProject.Name, result.Project.Name);

            Assert.Equal(newArtifactType.Id, result.ArtifactType.Id);
            Assert.Equal(newArtifactType.Name, result.ArtifactType.Name);
            Assert.Equal(newArtifactType.Description, result.ArtifactType.Description);

            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactWithId()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.Update(artifactToUpdate));
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoProjectWithId()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.Update(artifactToUpdate));
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ExceptionNoArtifactTypeWithId()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

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

            // Act/Assert
            await Assert.ThrowsAsync<System.ArgumentException>(() => _classUnderTest.Update(artifactToUpdate));
            _mapper.Verify(mock => mock.Map<Models.Artifact>(It.IsAny<Artifact>()), Times.Never);
        }

        // Delete
        [Fact]
        public async Task Delete_Returns()
        {
            // Arange
            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();
            var artifactType = CreateArtifactType();
            var project = CreateProject();
            var artifact = CreateArtifact(project, artifactType);

            // Act
            await _classUnderTest.Delete(artifact.Id);

            // Assert
            Assert.Null(await _dataAccess.Artifacts.FirstOrDefaultAsync(a => a.Id == artifact.Id));
        }
    }
}