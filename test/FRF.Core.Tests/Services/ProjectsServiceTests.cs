using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.DataAccess.EntityModels;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class ProjectsServiceTests
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private DataAccessContextForTest _dataAccess;
        private ProjectsService _classUnderTest;
        private readonly Mock<IUserService> _userService;

        public ProjectsServiceTests()
        {
            _configuration = new Mock<IConfiguration>();

            _userService = new Mock<IUserService>();

            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);

            _dataAccess.Database.EnsureDeleted();
            _dataAccess.Database.EnsureCreated();

            _classUnderTest = new ProjectsService(_dataAccess, _mapper, _userService.Object);
        }

        private DataAccess.EntityModels.Project CreateProject()
        {
            var project = new DataAccess.EntityModels.Project();
            project.Name = "[MOCK] Project name";
            project.Owner = "[MOCK] Project owner";
            project.Client = "[MOCK] Project client";
            project.Budget = 1000;
            project.CreatedDate = DateTime.Now;
            project.ProjectCategories = new List<DataAccess.EntityModels.ProjectCategory>();
            _dataAccess.Projects.Add(project);
            _dataAccess.SaveChanges();

            return project;
        }

        private DataAccess.EntityModels.Category CreateCategory()
        {
            var category = new DataAccess.EntityModels.Category();
            category.Name = "[MOCK] Category name";
            category.Description = "[MOCK] Category description";
            category.ProjectCategories = new List<DataAccess.EntityModels.ProjectCategory>();
            _dataAccess.Categories.Add(category);
            _dataAccess.SaveChanges();

            return category;
        }

        private UsersByProject CreateUserByProject(DataAccess.EntityModels.Project project)
        {
            var userByProject = new UsersByProject();
            userByProject.ProjectId = project.Id;
            userByProject.Project = project;
            userByProject.UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba");

            _dataAccess.UsersByProject.Add(userByProject);
            _dataAccess.SaveChanges();

            return userByProject;
        }

        private UsersProfile CreateUsersProfile()
        {
            return new UsersProfile()
            {
                Avatar = null,
                UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"),
                Email = "email@mock.moq",
                Fullname = "Mock User"
            };
        }
      [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            // Arange
            var project = CreateProject();
            var userByProject = CreateUserByProject(project);
            var userProfile = CreateUsersProfile();
            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<Guid>()))
                .ReturnsAsync(userProfile);

            // Act
            var result = await _classUnderTest.GetAllAsync(userByProject.UserId);

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Project>>>(result);
            Assert.True(result.Success);
            var resultValue = Assert.Single(result.Value);

            Assert.Equal(project.Id, resultValue.Id);
            Assert.Equal(project.Name, resultValue.Name);
            Assert.Equal(project.Owner, resultValue.Owner);
            Assert.Equal(project.Client, resultValue.Client);
            Assert.Equal(project.Budget, resultValue.Budget);
            Assert.Equal(project.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(project.ModifiedDate, resultValue.ModifiedDate);

            var usersProfiles = resultValue.UsersByProject.ToList();
            Assert.Equal(userProfile.Email, usersProfiles[0].Email);
            Assert.Equal(userProfile.UserId, usersProfiles[0].UserId);
            Assert.Equal(userProfile.Fullname, usersProfiles[0].Fullname);

            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList()
        {
            // Arange
            var userId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba");

            // Act
            var result = await _classUnderTest.GetAllAsync(userId);

            // Assert
            Assert.IsType<ServiceResponse<List<Models.Project>>>(result);
            Assert.True(result.Success);
            Assert.Empty(result.Value);
        }

       [Fact]
        public async Task GetAsync_ReturnsProject()
        {
            // Arange
            var project = CreateProject();
            var userByProject = CreateUserByProject(project);
            var userProfile = CreateUsersProfile();

            _userService
                .Setup(mock => mock.GetCurrentUserIdAsync())
                .ReturnsAsync(new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"));
            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<Guid>()))
                .ReturnsAsync(userProfile);

            // Act
            var result = await _classUnderTest.GetAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(project.Id, resultValue.Id);
            Assert.Equal(project.Name, resultValue.Name);
            Assert.Equal(project.Owner, resultValue.Owner);
            Assert.Equal(project.Client, resultValue.Client);
            Assert.Equal(project.Budget, resultValue.Budget);
            Assert.Equal(project.CreatedDate, resultValue.CreatedDate);
            Assert.Equal(project.ModifiedDate, resultValue.ModifiedDate);

            var usersProfiles = resultValue.UsersByProject.ToList();
            Assert.Equal(userProfile.Email, usersProfiles[0].Email);
            Assert.Equal(userProfile.UserId, usersProfiles[0].UserId);
            Assert.Equal(userProfile.Fullname, usersProfiles[0].Fullname);

            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<Guid>()), Times.Once);
            _userService.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsErrorNoProject()
        {
            // Arange
            var projectId = 0;

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }

        [Fact]
        public async Task SaveAsync_ReturnsProject()
        {
            // Arange
            var userByProject = new UsersProfile()
            {
                UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba")
            };

            var projectToSave = new Models.Project();
            projectToSave.Name = "[Mock] Project name 1";
            projectToSave.Owner = "[Mock] Project Owner";
            projectToSave.Client = "[Mock] Project Client";
            projectToSave.Budget = 1000;
            projectToSave.ProjectCategories = new List<Models.ProjectCategory>();
            projectToSave.UsersByProject = new List<UsersProfile>
            {
                userByProject
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(projectToSave.Name, resultValue.Name);
            Assert.Equal(projectToSave.Owner, resultValue.Owner);
            Assert.Equal(projectToSave.Client, resultValue.Client);
            Assert.Equal(projectToSave.Budget, resultValue.Budget);
            Assert.Null(resultValue.ModifiedDate);
            Assert.Equal(projectToSave.UsersByProject.ToList()[0].UserId, resultValue.UsersByProject.ToList()[0].UserId);
        }

        [Fact]
        public async Task SaveAsync_ReturnsErrorNoCategory()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);

            var category = new Models.Category
            {
                Id = 0,
                Name = "[Mock] Category Name"
            };

            var projectToSave = new Models.Project();
            projectToSave.Name = "[Mock] Project name 1";
            projectToSave.Owner = "[Mock] Project Owner";
            projectToSave.Client = "[Mock] Project Client";
            projectToSave.CreatedDate = DateTime.Now;

            var projectCategories = new Models.ProjectCategory();
            projectCategories.Category = category;

            projectToSave.ProjectCategories = new List<Models.ProjectCategory>
            {
                projectCategories
            };

            // Act
            var result = await _classUnderTest.SaveAsync(projectToSave);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ErrorCodes.CategoryNotExists);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsProject()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);
            var category = CreateCategory();

            var projectCategory = new Models.ProjectCategory()
            {
                Category = new Models.Category()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            };

            var userByProject = new UsersProfile()
            {
                UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba")
            };

            var projectToUpdate = new Models.Project();
            projectToUpdate.Id = project.Id;
            projectToUpdate.Name = "[Mock] Updated Project name";
            projectToUpdate.Owner = "[Mock] Updated Project Owner";
            projectToUpdate.Client = "[Mock] Updated Project Client";
            projectToUpdate.Budget = 50000;
            projectToUpdate.CreatedDate = project.CreatedDate;
            projectToUpdate.UsersByProject = new List<UsersProfile>
            {
                userByProject
            };
            projectToUpdate.ProjectCategories = new List<Models.ProjectCategory>()
            {
                projectCategory
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(projectToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.True(result.Success);
            var resultValue = result.Value;

            Assert.Equal(projectToUpdate.Name, resultValue.Name);
            Assert.Equal(projectToUpdate.Owner, resultValue.Owner);
            Assert.Equal(projectToUpdate.Client, resultValue.Client);
            Assert.Equal(projectToUpdate.Budget, resultValue.Budget);
            Assert.Equal(projectToUpdate.CreatedDate, resultValue.CreatedDate);
            Assert.NotNull(resultValue.ModifiedDate);
            Assert.Equal(projectToUpdate.UsersByProject.ToList()[0].UserId, resultValue.UsersByProject.ToList()[0].UserId);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsErrorNoCategory()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);

            var category = new Models.Category();
            category.Id = 0;
            category.Name = "[Mock] Category Name";

            var projectToUpdate = new Models.Project();
            projectToUpdate.Name = "[Mock] Project name 1";
            projectToUpdate.Owner = "[Mock] Project Owner";
            projectToUpdate.Client = "[Mock] Project Client";
            projectToUpdate.CreatedDate = project.CreatedDate;

            var projectCategories = new Models.ProjectCategory();
            projectCategories.Category = category;

            projectToUpdate.ProjectCategories = new List<Models.ProjectCategory>
            {
                projectCategories
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(projectToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ErrorCodes.CategoryNotExists);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsErrorNoProject()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);
            var category = CreateCategory();

            var projectCategory = new Models.ProjectCategory()
            {
                Category = new Models.Category()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            };

            var userByProject = new UsersProfile()
            {
                UserId = new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba")
            };

            var projectToUpdate = new Models.Project();
            projectToUpdate.Id = 0;
            projectToUpdate.Name = "[Mock] Project name 1";
            projectToUpdate.Owner = "[Mock] Project Owner";
            projectToUpdate.Client = "[Mock] Project Client";
            projectToUpdate.CreatedDate = project.CreatedDate;
            projectToUpdate.UsersByProject = new List<UsersProfile>
            {
                userByProject
            };
            projectToUpdate.ProjectCategories = new List<Models.ProjectCategory>()
            {
                projectCategory
            };

            // Act
            var result = await _classUnderTest.UpdateAsync(projectToUpdate);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsProject()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);
            _userService
                .Setup(mock => mock.GetCurrentUserIdAsync())
                .ReturnsAsync(new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"));

            // Act
            var result = await _classUnderTest.DeleteAsync(project.Id);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsErrorNoProject()
        {
            // Arange
            var projectId = 0;
            _userService
                .Setup(mock => mock.GetCurrentUserIdAsync())
                .ReturnsAsync(new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"));
            // Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            Assert.IsType<ServiceResponse<Models.Project>>(result);
            Assert.False(result.Success);
            Assert.Equal(result.Error.Code, ErrorCodes.ProjectNotExists);
        }
    }
}
