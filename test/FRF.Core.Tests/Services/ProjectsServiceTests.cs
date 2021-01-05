using AutoMapper;
using FRF.Core.Models;
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
            Assert.IsType<List<Models.Project>>(result);
            Assert.Single(result);
            Assert.Equal(project.Id, result[0].Id);
            Assert.Equal(project.Name, result[0].Name);
            Assert.Equal(project.Owner, result[0].Owner);
            Assert.Equal(project.Client, result[0].Client);
            Assert.Equal(project.Budget, result[0].Budget);
            Assert.Equal(project.CreatedDate, result[0].CreatedDate);
            Assert.Equal(project.ModifiedDate, result[0].ModifiedDate);

            var usersProfiles = result[0].UsersByProject.ToList();
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
            Assert.IsType<List<Models.Project>>(result);
            Assert.Empty(result);
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
            Assert.IsType<Models.Project>(result);

            Assert.Equal(project.Id, result.Id);
            Assert.Equal(project.Name, result.Name);
            Assert.Equal(project.Owner, result.Owner);
            Assert.Equal(project.Client, result.Client);
            Assert.Equal(project.Budget, result.Budget);
            Assert.Equal(project.CreatedDate, result.CreatedDate);
            Assert.Equal(project.ModifiedDate, result.ModifiedDate);

            var usersProfiles = result.UsersByProject.ToList();
            Assert.Equal(userProfile.Email, usersProfiles[0].Email);
            Assert.Equal(userProfile.UserId, usersProfiles[0].UserId);
            Assert.Equal(userProfile.Fullname, usersProfiles[0].Fullname);

            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<Guid>()), Times.Once);
            _userService.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsNull()
        {
            // Arange
            var projectId = 0;

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            Assert.Null(result);
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
            Assert.Equal(projectToSave.Name, result.Name);
            Assert.Equal(projectToSave.Owner, result.Owner);
            Assert.Equal(projectToSave.Client, result.Client);
            Assert.Equal(projectToSave.Budget, result.Budget);
            Assert.Null(result.ModifiedDate);
            Assert.Equal(projectToSave.UsersByProject.ToList()[0].UserId, result.UsersByProject.ToList()[0].UserId);
        }

        [Fact]
        public async Task SaveAsync_ReturnsNullNoCategory()
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
            Assert.Null(result);
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
            Assert.Equal(projectToUpdate.Name, result.Name);
            Assert.Equal(projectToUpdate.Owner, result.Owner);
            Assert.Equal(projectToUpdate.Client, result.Client);
            Assert.Equal(projectToUpdate.Budget, result.Budget);
            Assert.Equal(projectToUpdate.CreatedDate, result.CreatedDate);
            Assert.NotNull(result.ModifiedDate);
            Assert.Equal(projectToUpdate.UsersByProject.ToList()[0].UserId, result.UsersByProject.ToList()[0].UserId);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNullNoCategory()
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
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNullNoResult()
        {
            // Arange
            var project = CreateProject();
            CreateUserByProject(project);
            var category = CreateCategory();

            var projectCategory = new Models.ProjectCategory()
            {
                Category = new Models.Category()
                {
                    Id = 0,
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
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue()
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
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse()
        {
            // Arange
            var projectId = 0;
            _userService
                .Setup(mock => mock.GetCurrentUserIdAsync())
                .ReturnsAsync(new Guid("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba"));
            // Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            Assert.False(result);
        }
    }
}
