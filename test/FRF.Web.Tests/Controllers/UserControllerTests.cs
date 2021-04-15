using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Projects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly Mock<IUserService> _userService;

        private readonly UserController _classUnderTest;

        public UserControllerTests()
        {
            _userService = new Mock<IUserService>();
            _classUnderTest = new UserController(_userService.Object, _mapper);
        }

        private void AddUserToContext(HttpContext context, UsersProfile user)
        {
            var userPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
            }));

            context.User = userPrincipal;
        }

        [Fact]
        public async Task GetUserPublicProfileAsync_ReturnsOk()
        {
            // Arrange
            var userProfile = new UsersProfile
            {
                Email = "any@email.com",
                Fullname = "John Doe",
                UserId = new System.Guid()
            };

            var context = new ControllerContext 
            {
                HttpContext = new DefaultHttpContext() 
            };

            AddUserToContext(context.HttpContext, userProfile);

            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<Core.Models.UsersProfile>(userProfile));

            _classUnderTest.ControllerContext = context;

            // Act
            var result = await _classUnderTest.GetUserPublicProfileAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<UserProfileDTO>(okResult.Value);

            Assert.Equal(userProfile.Email, resultValue.Email);
            Assert.Equal(userProfile.Fullname, resultValue.Fullname);
            Assert.Equal(userProfile.UserId, resultValue.UserId);

            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserPublicProfileAsync_ReturnsBadRequest()
        {
            // Arrange
            var userProfile = new UsersProfile
            {
                Email = "any@email.com",
                Fullname = "John Doe",
                UserId = new System.Guid()
            };

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            AddUserToContext(context.HttpContext, userProfile);

            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<Core.Models.UsersProfile>(new Error(ErrorCodes.UserNotExists, "[Mock] Error Message")));

            _classUnderTest.ControllerContext = context;

            // Act
            var result = await _classUnderTest.GetUserPublicProfileAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SearchUserAsync_ReturnsOk()
        {
            // Arrange
            var userProfile = new UsersProfile
            {
                Email = "any@email.com",
                Fullname = "John Doe",
                UserId = new System.Guid()
            };

            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<Core.Models.UsersProfile>(userProfile));

            // Act
            var result = await _classUnderTest.SearchUserAsync(userProfile.Email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<UserProfileDTO>(okResult.Value);

            Assert.Equal(userProfile.Email, resultValue.Email);
            Assert.Equal(userProfile.Fullname, resultValue.Fullname);
            Assert.Equal(userProfile.UserId, resultValue.UserId);

            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SearchUserAsync_ReturnBadRequest()
        {
            // Arrange
            var userEmail = "";

            // Act
            var result = await _classUnderTest.SearchUserAsync(userEmail);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchUserAsync_ReturnsNotFound()
        {
            // Arrange
            var userProfile = new UsersProfile
            {
                Email = "any@email.com",
                Fullname = "John Doe",
                UserId = new System.Guid()
            };

            _userService
                .Setup(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()))
                .ReturnsAsync(new ServiceResponse<Core.Models.UsersProfile>(new Error(ErrorCodes.UserNotExists, "[Mock] Error Message")));

            // Act
            var result = await _classUnderTest.SearchUserAsync(userProfile.Email);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Once);
        }
    }
}
