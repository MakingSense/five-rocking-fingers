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
using CoreModels = FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.XmlValidation;
using System.Xml.Linq;
using System.Linq;

namespace FRF.Core.Tests.Services
{
    public class ArtifactsServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly DataAccessContextForTest _dataAccess;
        private readonly ArtifactsService _classUnderTest;
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

        private Artifact CreateArtifact()
        {
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var name = DateTime.Now.Millisecond;
            var artifact = new Artifact()
            {
                Name = $"[Mock] Artifact name {name}",
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

        private Artifact CreateArtifactWithSetting(Project project, ArtifactType artifactType, string settingName, int settingValue)
        {
            var random = new Random();
            var artifact = new Artifact()
            {
                Name = "[Mock] Artifact name " + random.Next(500),
                CreatedDate = DateTime.Now,
                Project = project,
                ProjectId = project.Id,
                ArtifactType = artifactType,
                ArtifactTypeId = artifactType.Id,
                Settings = new XElement("Settings", new XElement(settingName, settingValue, new XAttribute("type", SettingTypes.Decimal)))
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
                Artifact1Property = "Property1" + propertyId,
                Artifact2Property = "Property1" + propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        private DataAccess.EntityModels.ArtifactsRelation CreateArtifactRelation(int artifact1Id, int artifact2Id,
            string artifact1Property, string artifact2Property, int relationTypeId)
        {
            var artifactRelation = new DataAccess.EntityModels.ArtifactsRelation()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = artifact1Property,
                Artifact2Property = artifact2Property,
                RelationTypeId = relationTypeId
            };

            _dataAccess.ArtifactsRelation.Add(artifactRelation);
            _dataAccess.SaveChanges();

            return artifactRelation;
        }

        private CoreModels.ArtifactsRelation CreateArtifactsRelationModel(int artifact1Id, int artifact2Id, string setting1, string setting2)
        {
            var artifactRelation = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = setting1,
                Artifact2Property = setting2,
                RelationTypeId = 1
            };

            return artifactRelation;
        }

        private ArtifactsRelation CreateArtifactsRelationEntityModel(int artifact1Id, int artifact2Id, string property1, string property2)
        {
            var random = new Random();
            var artifactRelation = new ArtifactsRelation()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = property1,
                Artifact2Property = property2,
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
        public async Task SaveAsync_ReturnsExceptionInvalidArtifactSettings_NaturalNumberWrongValue1()
        {
            // Arange
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactToSave = new CoreModels.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;
            var setting1 = new XElement("setting1", 100);
            var setting2 = new XElement("setting2", -1);

            var relationalFields = new Dictionary<string, string>();
            relationalFields.Add(setting1.Name.ToString(), SettingTypes.Decimal);
            relationalFields.Add(setting2.Name.ToString(), SettingTypes.NaturalNumber);
            artifactToSave.RelationalFields = relationalFields;

            artifactToSave.Settings = new XElement("Settings", setting1, setting2);

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.Equal(ErrorCodes.InvalidArtifactSettings, result.Error.Code);
        }

        [Fact]
        public async Task SaveAsync_ReturnsExceptionInvalidArtifactSettings_NaturalNumberWrongValue2()
        {
            // Arange
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactToSave = new CoreModels.Artifact();
            artifactToSave.Name = "[Mock] Artifact name 1";
            artifactToSave.ProjectId = project.Id;
            artifactToSave.ArtifactTypeId = artifactType.Id;
            var setting1 = new XElement("setting1", 100);
            var setting2 = new XElement("setting2", 1.1);

            var relationalFields = new Dictionary<string, string>();
            relationalFields.Add(setting1.Name.ToString(), SettingTypes.Decimal);
            relationalFields.Add(setting2.Name.ToString(), SettingTypes.NaturalNumber);
            artifactToSave.RelationalFields = relationalFields;

            artifactToSave.Settings = new XElement("Settings", setting1, setting2);

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Save(artifactToSave);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
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
            artifact.Settings = new XElement("Settings", new XElement("Setting1", "Value1"));

            var providerAux = CreateProvider();
            var artifactTypeAux = CreateArtifactType(provider);
            var projectAux = CreateProject();
            var artifactAux = CreateArtifact(project, artifactType);
            artifactAux.Settings = new XElement("Settings", new XElement("Setting1Aux", "Value1"));

            var relation = new DataAccess.EntityModels.ArtifactsRelation
            {
                Artifact1Id = artifact.Id,
                Artifact1 = artifact,
                Artifact2Id = artifactAux.Id,
                Artifact2 = artifactAux,
                RelationTypeId = 0
            };

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
                Settings = new XElement("Settings", new XElement("Setting1Modified", "Value1"))
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
            Assert.True(XNode.DeepEquals(artifactToUpdate.Settings, resultValue.Settings));

            Assert.Empty(_dataAccess.ArtifactsRelation
                .Include(ar => ar.Artifact1)
                .Include(ar => ar.Artifact2)
                .Where(ar => ar.Artifact1Id == artifact.Id || ar.Artifact2Id == artifact.Id).ToList());

            Assert.Equal(newProject.Id, resultValue.Project.Id);
            Assert.Equal(newProject.Name, resultValue.Project.Name);

            Assert.Equal(newArtifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(newArtifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(newArtifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(newArtifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(newArtifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task UpdateAsync_ArtifactWithChainRelations_ReturnsArtifact()
        {
            // Arange
            var project = CreateProject();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);

            var setting1Name = "Setting1";
            var setting1Value = 100;

            var setting2Name = "Setting2";
            var setting2Value = 200;

            var setting3Name = "Setting3";
            var setting3Value = 300;

            var artifact1 = CreateArtifactWithSetting(project, artifactType, setting1Name, setting1Value);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, setting2Name, setting2Value);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, setting3Name, setting3Value);

            var relation12 = CreateArtifactRelation(artifact1.Id, artifact2.Id, setting1Name, setting2Name, 0);

            var relation23 = CreateArtifactRelation(artifact2.Id, artifact3.Id, setting2Name, setting3Name, 0);

            var updatedSetting1Value = 1000;

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifact1.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = _mapper.Map<CoreModels.Project>(project),
                ArtifactTypeId = artifactType.Id,
                ArtifactType = _mapper.Map<CoreModels.ArtifactType>(artifactType),
                Settings = new XElement("Settings", new XElement(setting1Name, updatedSetting1Value))
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.IsType<CoreModels.Artifact>(result.Value);

            var resultValueArtifact2 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact2.Id);
            var resultValueArtifact3 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact3.Id);
            Assert.Equal(updatedSetting1Value, int.Parse(resultValueArtifact2.Settings.Element(setting2Name).Value));
            Assert.Equal(updatedSetting1Value, int.Parse(resultValueArtifact3.Settings.Element(setting3Name).Value));

            Assert.Equal(artifactToUpdate.Id, resultValue.Id);
            Assert.Equal(artifactToUpdate.Name, resultValue.Name);
            Assert.Equal(artifact1.CreatedDate, resultValue.CreatedDate);
            Assert.NotEqual(artifactToUpdate.CreatedDate, resultValue.CreatedDate); // This because it shouldn't allow CreatedDate change
            Assert.NotNull(resultValue.ModifiedDate);
            Assert.Equal(artifactToUpdate.ProjectId, resultValue.ProjectId);
            Assert.Equal(artifactToUpdate.ArtifactTypeId, resultValue.ArtifactTypeId);
            Assert.True(XNode.DeepEquals(artifactToUpdate.Settings, resultValue.Settings));

            Assert.Equal(project.Id, resultValue.Project.Id);
            Assert.Equal(project.Name, resultValue.Project.Name);

            Assert.Equal(artifactType.Id, resultValue.ArtifactType.Id);
            Assert.Equal(artifactType.Name, resultValue.ArtifactType.Name);
            Assert.Equal(artifactType.Description, resultValue.ArtifactType.Description);

            Assert.Equal(artifactType.Provider.Id, resultValue.ArtifactType.Provider.Id);
            Assert.Equal(artifactType.Provider.Name, resultValue.ArtifactType.Provider.Name);
        }

        [Fact]
        public async Task UpdateAsync_RemovesRelationSettingTypeChanged()
        {
            // Arange
            var project = CreateProject();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);

            var settingArtifact1 = "setting1";
            var settingArtifact2 = "setting2";

            var artifactInDb = new Artifact();
            artifactInDb.Name = "[Mock] Artifact name 1";
            artifactInDb.ProjectId = project.Id;
            artifactInDb.ArtifactTypeId = artifactType.Id;
            artifactInDb.Settings = new XElement("Settings", new XElement(settingArtifact1, 100, new XAttribute("type", SettingTypes.NaturalNumber)));

            _dataAccess.Artifacts.Add(artifactInDb);
            _dataAccess.SaveChanges();

            var artifact2 = CreateArtifactWithSetting(project, artifactType, settingArtifact2, 100);

            CreateArtifactRelation(artifactInDb.Id, artifact2.Id, settingArtifact1, settingArtifact2, 0);

            var relationalFields = new Dictionary<string, string>();
            relationalFields.Add(settingArtifact1, SettingTypes.Decimal);

            var updatedArtifact = new CoreModels.Artifact()
            {
                Id = artifactInDb.Id,
                Name = artifactInDb.Name,
                ProjectId = artifactInDb.ProjectId,
                ArtifactTypeId = artifactInDb.ArtifactTypeId,
                Settings = new XElement("Settings", new XElement(settingArtifact1, 100)),
                RelationalFields = relationalFields
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Update(updatedArtifact);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.True(result.Success);
            Assert.Equal(0, _dataAccess.ArtifactsRelation.Count());
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
        public async Task UpdateAsync_ReturnsExceptionInvalidArtifactSettings2()
        {
            // Arange
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactInDb = new Artifact();
            artifactInDb.Name = "[Mock] Artifact name 1";
            artifactInDb.ProjectId = project.Id;
            artifactInDb.ArtifactTypeId = artifactType.Id;
            var setting1 = new XElement("setting1", 100, new XAttribute("type", SettingTypes.NaturalNumber));
            var setting2 = new XElement("setting2", 1, new XAttribute("type", SettingTypes.NaturalNumber));
            artifactInDb.Settings = new XElement("Settings", setting1, setting2);

            _dataAccess.Artifacts.Add(artifactInDb);
            _dataAccess.SaveChanges();

            var newSetting1 = new XElement(setting1.Name.ToString(), setting1.Value);
            var newSetting2 = new XElement(setting2.Name.ToString(), -1);
            var relationalFields = new Dictionary<string, string>();
            relationalFields.Add(newSetting1.Name.ToString(), SettingTypes.NaturalNumber);
            relationalFields.Add(newSetting2.Name.ToString(), SettingTypes.NaturalNumber);

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifactInDb.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = _mapper.Map<CoreModels.Project>(project),
                ArtifactTypeId = artifactType.Id,
                ArtifactType = _mapper.Map<CoreModels.ArtifactType>(artifactType),
                Settings = new XElement("Settings", newSetting1, newSetting2),
                RelationalFields = relationalFields                
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var result = await _classUnderTest.Update(artifactToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<CoreModels.Artifact>>(result);
            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorCodes.InvalidArtifactSettings, result.Error.Code);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsExceptionInvalidArtifactSettings3()
        {
            // Arange
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifactInDb = new Artifact();
            artifactInDb.Name = "[Mock] Artifact name 1";
            artifactInDb.ProjectId = project.Id;
            artifactInDb.ArtifactTypeId = artifactType.Id;
            var setting1 = new XElement("setting1", 1.8, new XAttribute("type", SettingTypes.Decimal));
            var setting2 = new XElement("setting2", 99.9, new XAttribute("type", SettingTypes.Decimal));
            artifactInDb.Settings = new XElement("Settings", setting1, setting2);

            _dataAccess.Artifacts.Add(artifactInDb);
            _dataAccess.SaveChanges();

            var newSetting1 = new XElement(setting1.Name.ToString(), setting1.Value);
            var newSetting2 = new XElement(setting2.Name.ToString(), setting2.Value);
            var relationalFields = new Dictionary<string, string>();
            relationalFields.Add(newSetting1.Name.ToString(), SettingTypes.Decimal);
            relationalFields.Add(newSetting2.Name.ToString(), SettingTypes.NaturalNumber);

            var artifactToUpdate = new CoreModels.Artifact()
            {
                Id = artifactInDb.Id,
                Name = "[Mock] Updated name",
                CreatedDate = DateTime.Now,
                ProjectId = project.Id,
                Project = _mapper.Map<CoreModels.Project>(project),
                ArtifactTypeId = artifactType.Id,
                ArtifactType = _mapper.Map<CoreModels.ArtifactType>(artifactType),
                Settings = new XElement("Settings", newSetting1, newSetting2),
                RelationalFields = relationalFields
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

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
        public async Task SetRelationAsync_ChainRelation_ReturnsListOfRelations()
        {
            // Arange
            var project = CreateProject();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);

            var setting1Name = "Setting1";
            var setting1Value = 100;

            var setting2Name = "Setting2";
            var setting2Value = 200;

            var setting3Name = "Setting3";
            var setting3Value = 300;

            var artifact1 = CreateArtifactWithSetting(project, artifactType, setting1Name, setting1Value);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, setting2Name, setting2Value);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, setting3Name, setting3Value);

            var relation12 = new Core.Models.ArtifactsRelation
            {
                Artifact1Id = artifact1.Id,
                Artifact1Property = setting1Name,
                Artifact2Id = artifact2.Id,
                Artifact2Property = setting2Name,
                RelationTypeId = 0
            };

            var relation23 = new ArtifactsRelation
            {
                Artifact1Id = artifact2.Id,
                Artifact1Property = setting2Name,
                Artifact2Id = artifact3.Id,
                Artifact2Property = setting3Name,
                RelationTypeId = 0
            };

            await _dataAccess.ArtifactsRelation.AddAsync(relation23);
            await _dataAccess.SaveChangesAsync();

            var relations = new List<Core.Models.ArtifactsRelation>
            {
                relation12
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifact1.Id, relations);
            var artifact2Response = await _classUnderTest.Get(artifact2.Id);
            var artifact3Response = await _classUnderTest.Get(artifact3.Id);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);
            var resultValueArtifact2 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact2.Id);
            var resultValueArtifact3 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact3.Id);
            Assert.Equal(setting1Value, int.Parse(resultValueArtifact2.Settings.Element(setting2Name).Value));
            Assert.Equal(setting1Value, int.Parse(resultValueArtifact3.Settings.Element(setting3Name).Value));
        }

        [Fact]
        public async Task SetRelationAsync_SettingWithTwoRelation_ReturnsListOfRelations()
        {
            // Arange
            var project = CreateProject();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);

            var setting1Name = "Setting1";
            var setting1Value = 100;

            var setting2Name = "Setting2";
            var setting2Value = 200;

            var setting3Name = "Setting3";
            var setting3Value = 300;

            var artifact1 = CreateArtifactWithSetting(project, artifactType, setting1Name, setting1Value);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, setting2Name, setting2Value);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, setting3Name, setting3Value);

            var relation13 = new Core.Models.ArtifactsRelation
            {
                Artifact1Id = artifact1.Id,
                Artifact1Property = setting1Name,
                Artifact2Id = artifact3.Id,
                Artifact2Property = setting3Name,
                RelationTypeId = 0
            };

            var relation23 = new ArtifactsRelation
            {
                Artifact1Id = artifact2.Id,
                Artifact1Property = setting2Name,
                Artifact2Id = artifact3.Id,
                Artifact2Property = setting3Name,
                RelationTypeId = 0
            };

            await _dataAccess.ArtifactsRelation.AddAsync(relation23);
            await _dataAccess.SaveChangesAsync();

            var relations = new List<Core.Models.ArtifactsRelation>
            {
                relation13
            };

            _settingsValidator.Setup(mock => mock.ValidateSettings(It.IsAny<CoreModels.Artifact>()))
                .Returns(true);

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifact1.Id, relations);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);

            var resultValueArtifact3 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact3.Id);


            AssertCompareArtifactRelation(relation13, resultValue[0]);
            Assert.Equal(setting1Value + setting2Value, int.Parse(resultValueArtifact3.Settings.Element(setting3Name).Value));
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenBaseArtifactNoExist()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();
            var baseArtifactId = int.MaxValue;

            var artifact = CreateArtifact();
            var artifactRelation = CreateArtifactsRelationModel(artifact.Id, baseArtifactId);
            artifactsRelationToSave.Add(artifactRelation);

            // Act
            var response = await _classUnderTest.SetRelationAsync(baseArtifactId, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.ArtifactNotExists, response.Error.Code);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyArtifactRepeated()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = _mapper.Map<Core.Models.Artifact>(CreateArtifact(project, artifactType));
            var i = 0;
            while (i < 3)
            {
                var artifact2 = CreateArtifact(project,artifactType);
                var artifactRelation = CreateArtifactsRelationModel(baseArtifact.Id, artifact2.Id);
                artifactsRelationToSave.Add(artifactRelation);
                i++;
            }
            artifactsRelationToSave.Add(artifactsRelationToSave[0]);
           
            // Act
            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotValid, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyRelationRepeated()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = _mapper.Map<Core.Models.Artifact>(CreateArtifact(project, artifactType));
            var artifact2 = CreateArtifact(project, artifactType);
            var artifactRelation = CreateArtifactsRelationModel(baseArtifact.Id, artifact2.Id);
            artifactsRelationToSave.Add(artifactRelation);
            
            var artifactRelationRepeated = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = artifact2.Id,
                Artifact2Id = baseArtifact.Id,
                Artifact1Property = artifactRelation.Artifact2Property,
                Artifact2Property = artifactRelation.Artifact1Property,
                RelationTypeId = 0
            };
            artifactsRelationToSave.Add(artifactRelationRepeated);

            // Act
            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotValid, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenHasAnyRelationWithoutBaseArtifact()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = _mapper.Map<Core.Models.Artifact>(CreateArtifact(project, artifactType));
            var i = 0;
            while (i < 3)
            {
                var artifact2 = CreateArtifact(project, artifactType);
                var artifactRelation = CreateArtifactsRelationModel(baseArtifact.Id, artifact2.Id);
                artifactsRelationToSave.Add(artifactRelation);
                i++;
            }
            var artifactRelationWithoutBaseArtifact = CreateArtifactsRelationModel(artifactsRelationToSave[0].Artifact2Id, artifactsRelationToSave[1].Artifact2Id);
            artifactsRelationToSave.Add(artifactRelationWithoutBaseArtifact);

            // Act

            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotValid, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenHasAnyArtifactFromAnotherProject()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = _mapper.Map<Core.Models.Artifact>(CreateArtifact(project, artifactType));
            var i = 0;
            while (i < 3)
            {
                var artifact2 = CreateArtifact(project, artifactType);
                var artifactRelation = CreateArtifactsRelationModel(baseArtifact.Id, artifact2.Id);
                artifactsRelationToSave.Add(artifactRelation);
                i++;
            }
            var notBaseProject = CreateProject();
            var artifactFromAnotherProject = CreateArtifact(notBaseProject, artifactType);
            var artifact2FromAnotherProject = CreateArtifact(notBaseProject, artifactType);
            var artifactRelationWithArtifactsFromAnotherProject = CreateArtifactsRelationModel(artifactFromAnotherProject.Id, artifact2FromAnotherProject.Id);
            artifactsRelationToSave.Add(artifactRelationWithArtifactsFromAnotherProject);

            // Act

            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.ArtifactFromAnotherProject, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsErrorCycleDetected1()
        {
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifact1 = CreateArtifactWithSetting(project, artifactType, "setting1", 100);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, "setting2", 200);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, "setting3", 300);

            var artifactRelation1 = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "setting1", "setting2");
            var artifactRelation2 = CreateArtifactsRelationModel(artifact2.Id, artifact3.Id, "setting2", "setting3");

            await _dataAccess.ArtifactsRelation.AddAsync(_mapper.Map<ArtifactsRelation>(artifactRelation1));
            await _dataAccess.ArtifactsRelation.AddAsync(_mapper.Map<ArtifactsRelation>(artifactRelation2));
            await _dataAccess.SaveChangesAsync();

            var newArtifactRelationList = new List<CoreModels.ArtifactsRelation>();

            newArtifactRelationList.Add(CreateArtifactsRelationModel(artifact3.Id, artifact1.Id, "setting3", "setting1"));

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifact1.Id, newArtifactRelationList);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.Equal(ErrorCodes.RelationCycleDetected, response.Error.Code);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsErrorCycleDetected2()
        {
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifact1 = CreateArtifact(project, artifactType);
            var SettingsArt1 = new XElement("Settings",
                new XElement("setting11", 100, new XAttribute("type", SettingTypes.Decimal)),
                new XElement("setting12", 200, new XAttribute("type", SettingTypes.Decimal)));
            artifact1.Settings = SettingsArt1;

            var artifact2 = CreateArtifact(project, artifactType);
            var SettingsArt2 = new XElement("Settings",
                new XElement("setting21", 300, new XAttribute("type", SettingTypes.Decimal)),
                new XElement("setting22", 400, new XAttribute("type", SettingTypes.Decimal)));
            artifact2.Settings = SettingsArt2;

            var artifactRelation1 = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "setting11", "setting21");
            var artifactRelation2 = CreateArtifactsRelationModel(artifact2.Id, artifact1.Id, "setting21", "setting12");
            var artifactRelation3 = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "setting12", "setting22");

            await _dataAccess.ArtifactsRelation.AddAsync(_mapper.Map<ArtifactsRelation>(artifactRelation1));
            await _dataAccess.ArtifactsRelation.AddAsync(_mapper.Map<ArtifactsRelation>(artifactRelation2));
            await _dataAccess.ArtifactsRelation.AddAsync(_mapper.Map<ArtifactsRelation>(artifactRelation3));
            await _dataAccess.SaveChangesAsync();

            var newArtifactRelationList = new List<CoreModels.ArtifactsRelation>();

            newArtifactRelationList.Add(CreateArtifactsRelationModel(artifact2.Id, artifact1.Id, "setting22", "setting11"));

            // Act
            var response = await _classUnderTest.SetRelationAsync(artifact1.Id, newArtifactRelationList);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.Equal(ErrorCodes.RelationCycleDetected, response.Error.Code);
        }
      
        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyBidirectionalRelationSameRelationType()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = CreateArtifactWithSetting(project, artifactType, "setting1", 100);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, "setting2", 200);

            var artifactRelation1 = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = baseArtifact.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "setting1",
                Artifact2Property = "setting2",
                RelationTypeId = 1
            };
            var artifactRelation2 = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = artifact2.Id,
                Artifact2Id = baseArtifact.Id,
                Artifact1Property = "setting2",
                Artifact2Property = "setting1",
                RelationTypeId = 1
            };
            artifactsRelationToSave.Add(artifactRelation2);
            await _dataAccess.ArtifactsRelation.AddAsync(
                _mapper.Map<ArtifactsRelation>(artifactRelation1));

            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationAlreadyExisted, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task SetRelationAsync_ReturnsNull_WhenIsAnyBidirectionalRelationDifferentRelationType()
        {
            // Arange
            var artifactsRelationToSave = new List<CoreModels.ArtifactsRelation>();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();
            var baseArtifact = CreateArtifactWithSetting(project, artifactType, "setting1", 100);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, "setting2", 200);

            var artifactRelation1 = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = baseArtifact.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "setting1",
                Artifact2Property = "setting2",
                RelationTypeId = 1
            };
            var artifactRelation2 = new CoreModels.ArtifactsRelation()
            {
                Artifact1Id = baseArtifact.Id,
                Artifact2Id = artifact2.Id,
                Artifact1Property = "setting1",
                Artifact2Property = "setting2",
                RelationTypeId = 0
            };
            artifactsRelationToSave.Add(artifactRelation2);
            await _dataAccess.ArtifactsRelation.AddAsync(
                _mapper.Map<ArtifactsRelation>(artifactRelation1));
            
            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.SetRelationAsync(baseArtifact.Id, artifactsRelationToSave);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationAlreadyExisted, response.Error.Code);
            Assert.Null(response.Value);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsNull_WhenIsAnyArtifactExcept()
        {
            // Arange
            var artifactsRelationUpdated = new List<CoreModels.ArtifactsRelation>();
            var artifactsRelationInDb = new List<ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var project = CreateProject();
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelation = CreateArtifactsRelationModel(artifact1Id, artifact2Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2");
                artifactsRelationUpdated.Add(artifactRelation);

                var artifactRelationDb = _mapper.Map<ArtifactsRelation>(artifactRelation);
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
            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var setting1Name = "Setting1";
            var setting1Value = 100;

            var setting2Name = "Setting2";
            var setting2Value = 200;

            var setting3Name = "Setting3";
            var setting3Value = 300;

            var artifact1 = CreateArtifactWithSetting(project, artifactType, setting1Name, setting1Value);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, setting2Name, setting2Value);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, setting3Name, setting3Value);

            var artifactRelation12 = CreateArtifactRelation(artifact1.Id, artifact2.Id, setting1Name, setting2Name, 1);
            var artifactRelation23 = CreateArtifactRelation(artifact2.Id, artifact3.Id, setting2Name, setting3Name, 1);

            var artifactRelation23Updated13 = new CoreModels.ArtifactsRelation()
            {
                Id = artifactRelation23.Id,
                Artifact1Id = artifact1.Id,
                Artifact2Id = artifact3.Id,
                Artifact1Property = setting1Name,
                Artifact2Property = setting3Name,
                RelationTypeId = 1
            };

            var artifactsRelationUpdated = new List<CoreModels.ArtifactsRelation>
            {
                artifactRelation23Updated13
            };

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifact2.Id, artifactsRelationUpdated);

            // Assert
            var resultValueArtifact1 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact1.Id);

            Assert.Equal(setting2Value + setting3Value, int.Parse(resultValueArtifact1.Settings.Element(setting1Name).Value));

            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);
            Assert.Equal(artifactsRelationUpdated[0].Artifact1Id, resultValue[0].Artifact1Id);
            Assert.Equal(artifactsRelationUpdated[0].Artifact2Id, resultValue[0].Artifact2Id);
            Assert.Equal(artifactsRelationUpdated[0].Artifact1Property, resultValue[0].Artifact1Property);
            Assert.Equal(artifactsRelationUpdated[0].Artifact2Property, resultValue[0].Artifact2Property);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsErrorRepeatedRelation()
        {
            // Arange
            var artifactsRelationUpdated = new List<CoreModels.ArtifactsRelation>();

            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);

            var artifactRelation1 = CreateArtifactsRelationEntityModel(artifact1.Id, artifact2.Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation1);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation2 = CreateArtifactsRelationEntityModel(artifact1.Id, artifact2.Id, "[Mock] Property artefact 1", "[Mock] Property artefact 3");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation2);
            await _dataAccess.SaveChangesAsync();

            var updatedRelation = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2");
            updatedRelation.Id = artifactRelation2.Id;
            artifactsRelationUpdated.Add(updatedRelation);

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifact1.Id, artifactsRelationUpdated);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.Equal(ErrorCodes.RelationNotValid, response.Error.Code);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsErrorCycleDetected1()
        {
            // Arange
            var artifactsRelationUpdated = new List<CoreModels.ArtifactsRelation>();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifact1 = CreateArtifactWithSetting(project, artifactType, "setting1", 100);
            var artifact2 = CreateArtifactWithSetting(project, artifactType, "setting2", 200);
            var artifact3 = CreateArtifactWithSetting(project, artifactType, "setting3", 300);
            var artifact4 = CreateArtifactWithSetting(project, artifactType, "setting4", 400);

            var artifactRelation1 = CreateArtifactsRelationEntityModel(artifact1.Id, artifact2.Id, "setting1", "setting2");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation1);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation2 = CreateArtifactsRelationEntityModel(artifact2.Id, artifact3.Id, "setting2", "setting3");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation2);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation3 = CreateArtifactsRelationEntityModel(artifact3.Id, artifact4.Id, "setting3", "setting4");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation3);
            await _dataAccess.SaveChangesAsync();

            var updatedRelation = CreateArtifactsRelationModel(artifact3.Id, artifact1.Id, "setting3", "setting1");
            updatedRelation.Id = artifactRelation3.Id;
            artifactsRelationUpdated.Add(updatedRelation);

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifact1.Id, artifactsRelationUpdated);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.Equal(ErrorCodes.RelationCycleDetected, response.Error.Code);
        }

        [Fact]
        public async Task UpdateRelationAsync_ReturnsErrorCycleDetected2()
        {
            // Arange
            var artifactsRelationUpdated = new List<CoreModels.ArtifactsRelation>();

            var provider = new Provider();
            provider.Name = ArtifactTypes.Custom;
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            var artifactType = CreateArtifactType(provider);
            var project = CreateProject();

            var artifact1 = CreateArtifact(project, artifactType);
            var SettingsArt1 = new XElement("Settings",
                new XElement("setting11", 100, new XAttribute("type", SettingTypes.Decimal)),
                new XElement("setting12", 200, new XAttribute("type", SettingTypes.Decimal)),
                new XElement("anotherSetting", 8000, new XAttribute("type", SettingTypes.Decimal)));
            artifact1.Settings = SettingsArt1;

            var artifact2 = CreateArtifact(project, artifactType);
            var SettingsArt2 = new XElement("Settings",
                new XElement("setting21", 300, new XAttribute("type", SettingTypes.Decimal)),
                new XElement("setting22", 400, new XAttribute("type", SettingTypes.Decimal)));
            artifact2.Settings = SettingsArt2;

            var artifactRelation1 = CreateArtifactsRelationEntityModel(artifact1.Id, artifact2.Id, "setting11", "setting21");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation1);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation2 = CreateArtifactsRelationEntityModel(artifact2.Id, artifact1.Id, "setting21", "setting12");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation2);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation3 = CreateArtifactsRelationEntityModel(artifact1.Id, artifact2.Id, "setting12", "setting22");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation3);
            await _dataAccess.SaveChangesAsync();

            var artifactRelation4 = CreateArtifactsRelationEntityModel(artifact2.Id, artifact1.Id, "setting22", "anotherSetting");
            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelation4);
            await _dataAccess.SaveChangesAsync();

            var updatedRelation = CreateArtifactsRelationModel(artifact2.Id, artifact1.Id, "setting22", "setting11");
            updatedRelation.Id = artifactRelation4.Id;
            artifactsRelationUpdated.Add(updatedRelation);

            // Act
            var response = await _classUnderTest.UpdateRelationAsync(artifact1.Id, artifactsRelationUpdated);

            // Assert
            Assert.False(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.Equal(ErrorCodes.RelationCycleDetected, response.Error.Code);
        }

        [Fact]
        public async Task GetAllRelationsByProjectIdAsync_ReturnsListOfRelations()
        {
            // Arange
            var project = CreateProject();
            var artifactsRelationInDb = new List<ArtifactsRelation>();
            var i = 0;
            while (i < 3)
            {
                var provider = CreateProvider();
                var artifactType = CreateArtifactType(provider);
                var artifact = CreateArtifact(project, artifactType);
                var artifact1Id = artifact.Id;
                var artifact2Id = artifact1Id++;

                var artifactRelationDb =
                    _mapper.Map<ArtifactsRelation>(
                        CreateArtifactsRelationModel(artifact1Id, artifact2Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2"));
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
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);
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
            var artifactRelation = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2");
            var artifactRelationMapped = _mapper.Map<ArtifactsRelation>(artifactRelation);

            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationMapped);
            await _dataAccess.SaveChangesAsync();

            // Act
            var response = await _classUnderTest.GetAllRelationsOfAnArtifactAsync(artifact1.Id);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);
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
            Assert.IsType<ServiceResponse<CoreModels.ArtifactsRelation>>(response);
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
            var setting1Name = "Setting1";
            var setting1Value = 100;
            var artifact1 = CreateArtifactWithSetting(project, artifactType, setting1Name, setting1Value);

            var setting2Name = "Setting2";
            var setting2Value = 200;
            var artifact2 = CreateArtifactWithSetting(project, artifactType, setting2Name, setting2Value);

            var setting3Name = "Setting2";
            var setting3Value = 300;
            var artifact3 = CreateArtifactWithSetting(project, artifactType, setting3Name, setting3Value);

            var artifactRelation12 = CreateArtifactRelation(artifact1.Id, artifact2.Id, setting1Name, setting2Name, 1);
            var artifactRelation13 = CreateArtifactRelation(artifact1.Id, artifact3.Id, setting1Name, setting3Name, 1);

            // Act
            var response = await _classUnderTest.DeleteRelationAsync(artifactRelation13.Id);

            // Assert
            var resultValueArtifact1 = await _dataAccess.Artifacts.SingleOrDefaultAsync(a => a.Id == artifact1.Id);

            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<CoreModels.ArtifactsRelation>>(response);
            var resultValue = Assert.IsAssignableFrom<CoreModels.ArtifactsRelation>(response.Value);
            Assert.Equal(artifactRelation13.Id, resultValue.Id);
            Assert.Equal(artifactRelation13.Artifact1Id, resultValue.Artifact1Id);
            Assert.Equal(artifactRelation13.Artifact2Id, resultValue.Artifact2Id);
            Assert.Equal(artifactRelation13.RelationTypeId, resultValue.RelationTypeId);
            Assert.Equal(artifactRelation13.Artifact1Property, resultValue.Artifact1Property);
            Assert.Equal(artifactRelation13.Artifact2Property, resultValue.Artifact2Property);
            Assert.Equal(setting2Value, int.Parse(resultValueArtifact1.Settings.Element(setting1Name).Value));
        }

        [Fact]
        public async Task DeleteRelationsAsync_ExceptionRelationNotExists()
        {
            // Arange
            var artifactRelationIds = new List<Guid>
            {
                new Guid()
            };

            // Act
            var response = await _classUnderTest.DeleteRelationsAsync(artifactRelationIds);

            // Assert
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            Assert.False(response.Success);
            Assert.NotNull(response.Error);
            Assert.Equal(ErrorCodes.RelationNotExists, response.Error.Code);
        }

        [Fact]
        public async Task DeleteRelationsAsync_ReturnsRelations()
        {
            // Arange
            var project = CreateProject();
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var artifact1 = CreateArtifact(project, artifactType);
            var artifact2 = CreateArtifact(project, artifactType);
            var artifactRelation = CreateArtifactsRelationModel(artifact1.Id, artifact2.Id, "[Mock] Property artefact 1", "[Mock] Property artefact 2");
            var artifactRelationMapped = _mapper.Map<FRF.DataAccess.EntityModels.ArtifactsRelation>(artifactRelation);

            await _dataAccess.ArtifactsRelation.AddAsync(artifactRelationMapped);
            await _dataAccess.SaveChangesAsync();

            var artifactRelationsIds = new List<Guid>
            {
                artifactRelationMapped.Id
            };

            // Act
            var response = await _classUnderTest.DeleteRelationsAsync(artifactRelationsIds);

            // Assert
            Assert.True(response.Success);
            Assert.IsType<ServiceResponse<IList<CoreModels.ArtifactsRelation>>>(response);
            var resultValue = Assert.IsAssignableFrom<IList<CoreModels.ArtifactsRelation>>(response.Value);
            Assert.Equal(artifactRelationMapped.Id, resultValue[0].Id);
            Assert.Equal(artifactRelation.Artifact1Id, resultValue[0].Artifact1Id);
            Assert.Equal(artifactRelation.Artifact2Id, resultValue[0].Artifact2Id);
            Assert.Equal(artifactRelation.RelationTypeId, resultValue[0].RelationTypeId);
            Assert.Equal(artifactRelation.Artifact1Property, resultValue[0].Artifact1Property);
            Assert.Equal(artifactRelation.Artifact2Property, resultValue[0].Artifact2Property);
        }

        internal void AssertCompareArtifactRelation(CoreModels.ArtifactsRelation expected, CoreModels.ArtifactsRelation actual)
        {
            Assert.Equal(expected.Artifact1Id, actual.Artifact1Id);
            Assert.Equal(expected.Artifact2Id, actual.Artifact2Id);
            Assert.Equal(expected.Artifact1Property, actual.Artifact1Property);
            Assert.Equal(expected.Artifact2Property, actual.Artifact2Property);
            Assert.Equal(expected.RelationTypeId, actual.RelationTypeId);
        }
    }
}
