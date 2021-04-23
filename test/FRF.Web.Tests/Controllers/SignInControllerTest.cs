using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.Core.Services;
using FRF.Web.Controllers;
using FRF.Web.Dtos.Projects;
using FRF.Web.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Controllers
{
    public class SignInControllerTest
    {
        private readonly Mock<IUserService> _userService;
        private readonly Mock<ISignInService> _signInService;
        private readonly IMapper _mapper = MapperBuilder.Build();
        private readonly SignInController _classUnderTest;

        public SignInControllerTest()
        {
            _signInService = new Mock<ISignInService>();
            _userService = new Mock<IUserService>();
            _classUnderTest = new SignInController(_signInService.Object, _mapper, _userService.Object);
        }

        private SignInDTO CreateSignInDto()
        {
            return new SignInDTO
            {
                Email = "mock@email.moq",
                Password = "Mock1234.",
                RememberMe = false
            };
        }

        [Fact]
        public async Task SignIn_WhenAreValidCredentials_ReturnUserToken()
        {
            // Arrange
            var signInDto = CreateSignInDto();
            var userSessionToken = "mock.session.token";

            _signInService
                .Setup(mock => mock.SignInAsync(It.IsAny<UserSignIn>()))
                .ReturnsAsync(new ServiceResponse<string>(userSessionToken));

            // Act
            var response = await _classUnderTest.SignIn(signInDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(userSessionToken, okResult.Value);
            _signInService.Verify(mock => mock.SignInAsync(It.IsAny<UserSignIn>()), Times.Once);
        }

        [Fact]
        public async Task SignIn_WhenAreInvalidCredentials_ReturnUnauthorized()
        {
            // Arrange
            var signInDto = CreateSignInDto();
            var error = ErrorCodes.InvalidCredentials;

            _signInService
                .Setup(mock => mock.SignInAsync(It.IsAny<UserSignIn>()))
                .ReturnsAsync(new ServiceResponse<string>(new Error(error, "Mock Error Message")));

            // Act
            var response = await _classUnderTest.SignIn(signInDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(response);
            _signInService.Verify(mock => mock.SignInAsync(It.IsAny<UserSignIn>()), Times.Once);
            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SignIn_WhenAuthenticationServerCurrentlyUnavailable_ReturnInternalServerError()
        {
            // Arrange
            var signInDto = CreateSignInDto();
            var error = ErrorCodes.AuthenticationServerCurrentlyUnavailable;

            _signInService
                .Setup(mock => mock.SignInAsync(It.IsAny<UserSignIn>()))
                .ReturnsAsync(new ServiceResponse<string>(new Error(error, "Mock Error Message")));

            // Act
            var result = await _classUnderTest.SignIn(signInDto);

            // Assert
            var internalServerError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerError.StatusCode);
            _signInService.Verify(mock => mock.SignInAsync(It.IsAny<UserSignIn>()), Times.Once);
            _userService.Verify(mock => mock.GetUserPublicProfileAsync(It.IsAny<string>()), Times.Never);
        }
    }
}