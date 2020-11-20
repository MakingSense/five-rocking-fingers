using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FiveRockingFingers.Controllers;
using FRF.Core.Models;
using FRF.Core.Services;
using FRF.Web.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
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
                .Setup(mock => mock.Map<ProjectDto>(It.IsAny<Project>()))
                .Returns(CreateProjectDto(project));

            // Act
            var result = await _classUnderTest.Get(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDto>(okResult.Value);

            Assert.Equal(project.Id, returnValue.Id);
            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.Is<Project>(c => c.Name == project.Name)), Times.Once);
        }

        [Fact]
        public async Task Get_NotFound_WhenProjectIsNotFound()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);

            // Act
            var result = await _classUnderTest.Get(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(projectId), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.Is<Project>(c => c.Name == project.Name)), Times.Never);
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
                .Setup(mock => mock.GetAllAsync(It.IsAny<string>()))
                .ReturnsAsync(listOfMockProject);

            var listOfMockProjectDto = listOfMockProject.Select(CreateProjectDto)
                .ToList();

            _mapper
                .Setup(mock => mock.Map<IEnumerable<ProjectDto>>(It.IsAny<List<Project>>()))
                .Returns(listOfMockProjectDto);
            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectDto>>(okResult.Value);

            Assert.Equal(sizeOfList, returnValue.Count);
            //_userService.Verify(mock => mock.GetCurrentUserId(), Times.Once);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<string>()), Times.Once);
            _mapper.Verify(
                mock => mock.Map<IEnumerable<ProjectDto>>(It.Is<List<Project>>(p =>
                    p.Count == listOfMockProjectDto.Count)), Times.Once);
        }

        [Fact]
        public async Task GetAll_NoContent_WhenAUserHasNoProjects()
        {
            // Arrange
            //TODO: AWS Credentials, Loggin bypassed. Uncomment after do:
            //_userService.Setup(mock => mock.GetCurrentUserId()).ReturnsAsync("c3c0b740-1c8f-49a0-a5d7-2354cb9b6eba");

            _projectsService
                .Setup(mock => mock.GetAllAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);
            // Act
            var result = await _classUnderTest.GetAll();

            // Assert
            var noContent = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(noContent.StatusCode, StatusCodes.Status204NoContent);

            //_userService.Verify(mock => mock.GetCurrentUserId(), Times.Once);
            _projectsService.Verify(mock => mock.GetAllAsync(It.IsAny<string>()), Times.Once);
            _mapper.Verify(mock => mock.Map<IEnumerable<ProjectDto>>(It.IsAny<List<Project>>()), Times.Never);
        }

        [Fact]
        public async Task Save_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectDto(project);

            _mapper
                .Setup(mock => mock.Map<Project>(It.IsAny<ProjectDto>()))
                .Returns(project);
            _projectsService
                .Setup(mock => mock.SaveAsync(It.IsAny<Project>()))
                .ReturnsAsync(project);
            _mapper
                .Setup(mock => mock.Map<ProjectDto>(It.IsAny<Project>())).Returns(projectUpsertDTO);
            // Act
            var result = await _classUnderTest.Save(projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDto>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.SaveAsync(project), Times.Once);
            _mapper.Verify(mock => mock.Map<Project>(It.Is<ProjectDto>(p => p.Owner == project.Owner)),
                Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.Is<Project>(p => p.Owner == project.Owner)),
                Times.Once);
        }

        [Fact]
        public async Task Save_ReturnsBadRequest_WhenProjectUpsertDTOIsInvalid()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectDto(project);

            // Act
            var result = await _classUnderTest.Save(projectUpsertDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            _projectsService.Verify(mock => mock.SaveAsync(project), Times.Never);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.IsAny<Project>()), Times.Never);
        }

        [Fact]
        public async Task Update_BadRequest_WhenGivenIdIsDifferentFromProjectId()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectDto(project);


            // Act
            var result = await _classUnderTest.Update(9, projectUpsertDTO);

            // Assert
            Assert.IsType<BadRequestResult>(result);

            _projectsService.Verify(mock => mock.UpdateAsync(It.IsAny<Project>()), Times.Never);
            _projectsService.Verify(mock => mock.GetAsync(It.IsAny<int>()), Times.Never);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.IsAny<Project>()),
                Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsOk()
        {
            // Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);
            var project = CreateProject(projectId);
            var projectUpsertDTO = CreateProjectDto(project);
            _mapper
                .Setup(mock => mock.Map<Project>(It.IsAny<ProjectDto>()))
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
                .Setup(mock => mock.Map<ProjectDto>(It.IsAny<Project>())).Returns(CreateProjectDto(project));
            // Act
            var result = await _classUnderTest.Update(projectId, projectUpsertDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectDto>(okResult.Value);

            Assert.Equal(project.Name, returnValue.Name);
            Assert.Equal(project.Budget, returnValue.Budget);

            _projectsService.Verify(mock => mock.UpdateAsync(project), Times.Once);
            _mapper.Verify(mock => mock.Map<ProjectDto>(It.Is<Project>(p => p.Owner == project.Owner)),
                Times.Once);
        }

        [Fact]
        public async Task Delete_NoContent_WhenProjectIsDeleted()
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
            var result = await _classUnderTest.Delete(projectId);
            //Assert
            Assert.IsType<NoContentResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }

        [Fact]
        public async Task Delete_NotFound_WhenProjectDoesntExist()
        {
            //Arrange
            var rnd = new Random();
            var projectId = rnd.Next(1, 99999);

            //Act
            var result = await _classUnderTest.Delete(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_NotFound_WhenProjectIsNotDeleted()
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
            var result = await _classUnderTest.Delete(projectId);

            // Assert
            Assert.IsType<NotFoundResult>(result);

            _projectsService.Verify(mock => mock.GetAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
            _projectsService.Verify(mock => mock.DeleteAsync(It.Is<int>(p => p.Equals(projectId))), Times.Once);
        }

        /// <summary>
        ///     Create mock Project object with a given id
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

        private ProjectDto CreateProjectDto(Project project)
        {
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Budget = project.Budget,
                Client = project.Client,
                Name = project.Name,
                Owner = project.Owner,
                CreatedDate = project.CreatedDate
            };
            return projectDto;
        }
    }
}