using AutoMapper;
using FiveRockingFingers.Controllers;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class ProjectsControllerTests
    {
        private readonly ProjectsController _classUnderTest;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IProjectsService> _projectsService;
        private readonly Mock<IUserService> _userService;

        public ProjectsControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _projectsService = new Mock<IProjectsService>();
            _userService = new Mock<IUserService>();
            _configuration = new Mock<IConfiguration>();

            _classUnderTest = new ProjectsController(_projectsService.Object, _mapper.Object, _userService.Object,
                _configuration.Object);
        }

        /// <summary>
        /// Create mock Project object with a given id
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <returns>A new Project object</returns>
        private Project CreateProject(int projectId)
        {
            var projectCategories = new Mock<ProjectCategory>();
            var userByProject = new Mock<UsersByProject>();
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
                UsersByProject = new List<UsersByProject>
                {
                    userByProject.Object
                }
            };

            return project;
        }

        private ProjectDTO CreateProjectDto(Project project)
        {
            var projectCategories = new Mock<ProjectCategoryDTO>();
            var userByProject = new Mock<UserProfile>();
            var projectDto = new ProjectDTO
            {
                Id = project.Id,
                Budget = project.Budget,
                Client = project.Client,
                Name = project.Name,
                Owner = project.Owner,
                CreatedDate = project.CreatedDate,
                ProjectCategories = new List<ProjectCategoryDTO>
                {
                    projectCategories.Object
                },
                UsersProfile = new List<UserProfile>
                {
                    userByProject.Object
                }
            };
            return projectDto;
        }

        private ProjectUpsertDTO CreateProjectUpsertDto(Project project)
        {
            var projectCategories = new Mock<ProjectCategoryDTO>();
            var userByProject = new Mock<UserProfileUpsert>();
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
                UsersProfile = new List<UserProfileUpsert>
                {
                    userByProject.Object
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
                .ReturnsAsync(project);
            _mapper
                .Setup(mock => mock.Map<ProjectDTO>(It.IsAny<Project>()))
                .Returns(CreateProjectDto(project));

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Id, returnValue.Id);
            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDTO>(It.Is<Project>(c => c.Name == project.Name)), Times.Once);
        }

        [Fact]
        public async Task Get_WhenProjectIsNotFound_NotFound()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);

            // Act
            var result = await _classUnderTest.GetAsync(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDTO>(It.Is<Project>(c => c.Name == project.Name)), Times.Never);
        }

        [Fact]
        public async Task GetAll_ReturnsOkAndListOfProjects()
        {
            // Arrange
            var sizeOfList = 5;
            var listOfMockProject = new List<Project>();
            for (var i = 0; i < sizeOfList; i++) listOfMockProject.Add(CreateProject(i));

            //TODO: AWS Credentials, Loggin bypassed. Uncomment after do:
            //_userService.Setup(mock => mock.GetCurrentUserId()).ReturnsAsync("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba");

            _projectsService
                .Setup(mock => mock.GetAllAsync(It.IsAny<Guid>()))
                .ReturnsAsync(listOfMockProject);

            var listOfMockProjectDto = listOfMockProject.Select(CreateProjectDto)
                .ToList();

            _mapper
                .Setup(mock => mock.Map<IEnumerable<ProjectDTO>>(It.IsAny<List<Project>>()))
                .Returns(listOfMockProjectDto);
            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDTO>>(okResult.Value);

            Assert.Equal(sizeOfList, returnValue.Count);
            //_userService.Verify(mock => mock.GetCurrentUserId(), Times.Once);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<Guid>()), Times.Once);
            _mapper.Verify(
                mock => mock.Map<IEnumerable<ProjectDTO>>(It.Is<List<Project>>(p =>
                    p.Count == listOfMockProjectDto.Count)), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenAUserHasNoProjects_ReturnOk()
        {
            // Arrange
            var listOfMockProjectEmpty = new List<Project>();
            var listOfMockProjectDtoEmpty = listOfMockProjectEmpty.Select(CreateProjectDto)
                .ToList();

            _projectsService
                .Setup(mock => mock.GetAllAsync(It.IsAny<Guid>()))
                .ReturnsAsync(listOfMockProjectEmpty);
            _mapper
                .Setup(mock => mock.Map<IEnumerable<ProjectDTO>>(It.IsAny<List<Project>>()))
                .Returns(listOfMockProjectDtoEmpty);
            // Act
            var result = await _classUnderTest.GetAllAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDTO>>(okResult.Value);

            //_userService.Verify(mock => mock.GetCurrentUserId(), Times.Once);
            Assert.Empty(returnValue);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<Guid>()), Times.Once);
            _mapper.Verify(
                mock => mock.Map<IEnumerable<ProjectDTO>>(It.Is<List<Project>>(p =>
                    p.Count == 0)), Times.Once);
        }

        [Fact]
        public async Task Save_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectDTO = CreateProjectDto(project);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            _mapper
                .Setup(mock => mock.Map<Project>(It.IsAny<ProjectUpsertDTO>()))
                .Returns(project);
            _projectsService
                .Setup(mock => mock.SaveAsync(It.IsAny<Project>()))
                .ReturnsAsync(project);
            _mapper
                .Setup(mock => mock.Map<ProjectDTO>(It.IsAny<Project>())).Returns(projectDTO);
            // Act
            var result = await _classUnderTest.SaveAsync(projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.SaveAsync(project), Times.Once);
            _mapper.Verify(mock => mock.Map<Project>(It.Is<ProjectUpsertDTO>(p => p.Owner == project.Owner)),
                Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDTO>(It.Is<Project>(p => p.Owner == project.Owner)),
                Times.Once);
        }

        [Fact]
        public async Task Save_WhenProjectUpsertDTOIsInvalid_ReturnsBadRequest()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);

            // Act
            var result = await _classUnderTest.SaveAsync(projectUpsertDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            _projectsService.Verify(mock => mock.SaveAsync(project), Times.Never);
            _mapper.Verify(mock => mock.Map<ProjectDTO>(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenGivenIdIsDifferentFromProjectId_BadRequest()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectUpsertDto(project);


            // Act
            var result = await _classUnderTest.UpdateAsync(9, projectUpsertDTO);

            // Assert
            Assert.IsType<NotFoundResult>(result);

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
            _mapper
                .Setup(mock => mock.Map<Project>(It.IsAny<ProjectDTO>()))
                .Returns(project);
            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(project);
            _mapper
                .Setup(mock => mock.Map(projectUpsertDTO, project));
            _projectsService
                .Setup(mock => mock.UpdateAsync(It.IsAny<Project>()))
                .ReturnsAsync(project);
            _mapper
                .Setup(mock => mock.Map<ProjectDTO>(It.IsAny<Project>())).Returns(CreateProjectDto(project));
            // Act
            var result = await _classUnderTest.UpdateAsync(projectId, projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDTO>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.UpdateAsync(project), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDTO>(It.Is<Project>(p => p.Owner == project.Owner)),
                Times.Once);
        }

        [Fact]
        public async Task Delete_WhenProjectIsDeleted_NoContent()
        {
            //Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            _projectsService
                .Setup(mock => mock.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(project);
            _projectsService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(true);
            //Act
            var result = await _classUnderTest.DeleteAsync(projectId);
            //Assert
            Assert.IsType<NoContentResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenProjectDoesntExist_NotFound()
        {
            //Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);

            //Act
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
                .ReturnsAsync(project);
            _projectsService
                .Setup(mock => mock.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(false);
            //Act
            var result = await _classUnderTest.DeleteAsync(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }
    }
}