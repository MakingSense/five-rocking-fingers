using AutoMapper;
using FiveRockingFingers.Controllers;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Dtos;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly ProjectsController _classUnderTest;
        private readonly Mock<IProjectsService> _projectsService;
        private readonly Mock<IUserService> _userService;
        private readonly IMapper _mapper = MapperBuilder.Build();

        public ProjectsControllerTests()
        {
            _projectsService = new Mock<IProjectsService>();
            _userService = new Mock<IUserService>();

            _classUnderTest = new ProjectsController(_projectsService.Object,
                _mapper,
                _userService.Object);
        }

        /// <summary>
        /// Create mock Project object with a given id
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <returns>A new Project object</returns>
        private Project CreateProject(int projectId)
        {
            var projectCategories = new Mock<ProjectCategory>();
            var user = new Mock<UsersProfile>();
            var project = new Project
            {
                Budget = 900,
                Client = "Cliente Prueba",
                CreatedDate = DateTime.Now,
                Id = projectId,
                Name = "prueba",
                Owner = "Making Sense",
                ModifiedDate = null,
                ProjectCategories = new List<ProjectCategory>
                {
                    projectCategories.Object
                },
                UsersByProject = new List<UsersProfile>
                {
                    user.Object
                }
            };

            return project;
        }

        private ProjectUpsertDTO CreateProjectUpsertDto(Project project)
        {
            var projectCategories = new Mock<ProjectCategoryDTO>();
            var user = new Mock<UserProfileUpsertDTO>();
            var projectUpsertDTO = new ProjectUpsertDTO
            {
                Budget = project.Budget,
                Client = project.Client,
                Name = project.Name,
                Owner = project.Owner,
                ProjectCategories = new List<ProjectCategoryDTO>
                {
                    projectCategories.Object
                },
                Users = new List<UserProfileUpsertDTO>
                {
                    user.Object
                }
            };
            return projectUpsertDTO;
        }

        [Fact]
        public async Task Get_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Id, returnValue.Id);
            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task Get_WhenProjectIsNotFound_NotFound()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(new Error(ErrorCodes.ProjectNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnsOkAndListOfProjects()
        {
            // Arrange
            var sizeOfList = 5;
            var listOfMockProject = new List<Project>();
            for (var i = 0; i < sizeOfList; i++) listOfMockProject.Add(CreateProject(i));
            
            _projectsService
                .Setup(mock => mock.GetAllAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ServiceResponse<List<Project>>(listOfMockProject));

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDTO>>(okResult.Value);

            Assert.Equal(sizeOfList, returnValue.Count);
            //_userService.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenAUserHasNoProjects_ReturnOk()
        {
            // Arrange
            var listOfMockProjectEmpty = new List<Project>();

            _projectsService
                .Setup(mock => mock.GetAllAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ServiceResponse<List<Project>>(listOfMockProjectEmpty));

            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDTO>>(okResult.Value);

            //_userService.Verify(mock => mock.GetCurrentUserIdAsync(), Times.Once);
            Assert.Empty(returnValue);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Save_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            _projectsService
                .Setup(mock => mock.SaveAsync(It.IsAny<Project>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            // Act
            var result = await _classUnderTest.SaveAsync(projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.SaveAsync(It.Is<Project>(p =>
                p.Name == returnValue.Name
                && p.Budget == returnValue.Budget)), Times.Once);
        }

        [Fact]
        public async Task Update_WhenProjectDoesntExist_BadRequest()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(new Error(ErrorCodes.ProjectNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.UpdateAsync(project.Id + 1, projectUpsertDTO);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.UpdateAsync(It.IsAny<Project>()), Times.Never);
            _projectsService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenGivenEmptyUserList_BadRequest()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            projectUpsertDTO.Users.Clear();

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            // Act
            var result = await _classUnderTest.UpdateAsync(project.Id, projectUpsertDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            _projectsService.Verify(mock => mock.UpdateAsync(It.IsAny<Project>()), Times.Never);
            _projectsService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            _projectsService
                .Setup(mock => mock.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            // Act
            var result = await _classUnderTest.UpdateAsync(projectId, projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.UpdateAsync(project), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenProjectIsDeleted_NoContent()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            _projectsService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            // Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Project>(okResult.Value);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenProjectDoesntExist_NotFound()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(new Error(ErrorCodes.ProjectNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenProjectIsNotDeleted_NotFound()
        {
            //Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);

            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(project));

            _projectsService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new ServiceResponse<Project>(new Error(ErrorCodes.ProjectNotExists, "Error message")));

            // Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }
    }
}