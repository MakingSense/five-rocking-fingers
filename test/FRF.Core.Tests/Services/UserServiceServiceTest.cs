using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using FRF.Core.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class UserServiceServiceTest : CognitoTestBase
    {
        private readonly UserService _classUnderTest;

        public UserServiceServiceTest()
        {
            _classUnderTest = new UserService(_userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async Task GetCurrentUserId_WhenIsNotLogin_ReturnsEmptyID()
        {
            // Arrange
            _signInManagerMock.Object.Context = new DefaultHttpContext();

            // Act
            var result = await _classUnderTest.GetCurrentUserId();

            // Assert
            Assert.IsType<Guid>(result);
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task GetCurrentUserId_ReturnsCurrentId()
        {
            // Arrange
            var cognitoUser = CreateCognitoUser();
            var context = MockContext(cognitoUser);

            _signInManagerMock.Object.Context = context;
            _userManagerMock
                .Setup(mock => mock.GetUserAsync(context.User))
                .ReturnsAsync(cognitoUser)
                .Verifiable();

            // Act
            var result = await _classUnderTest.GetCurrentUserId();

            // Assert
            Assert.IsType<Guid>(result);
            Assert.Equal(cognitoUser.UserID, result.ToString());
            _signInManagerMock.Verify();
            _userManagerMock.Verify(mock => mock.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        [Fact]
        public async Task GetUserPublicProfile_WhenEmailNotFound_ReturnsNull()
        {
            // Arrange
            CognitoUser cognitoUser = null;
            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);
            // Act
            var result = await _classUnderTest.GetUserPublicProfile(It.IsAny<string>());
            // Assert
            Assert.Null(result);
            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetUserPublicProfile_ReturnsUser()
        {
            // Arrange
            var cognitoUser = CreateCognitoUser();
            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);

            // Act
            var result = await _classUnderTest.GetUserPublicProfile(cognitoUser.Attributes["email"]);

            // Assert
            Assert.IsType<UsersProfile>(result);
            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);

            Assert.Equal(cognitoUser.Attributes["email"], result.Email);
            Assert.Equal($"{cognitoUser.Attributes["name"]} {cognitoUser.Attributes["family_name"]}", result.Fullname);
            Assert.IsType<Guid>(result.UserId);
            Assert.Equal(cognitoUser.UserID, result.UserId.ToString());
        }

        [Fact]
        public async Task GetUserPublicProfile_WhenUserIDNotFound_ReturnsNull()
        {
            // Arrange
            CognitoUser cognitoUser = null;
            _userManagerMock
                .Setup(mock => mock.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);
            // Act
            var result = await _classUnderTest.GetUserPublicProfile(It.IsAny<string>());
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserPublicProfile_WhenUserIdIdFound_ReturnsUser()
        {
            // Arrange
            var cognitoUser = CreateCognitoUser();
            _userManagerMock
                .Setup(mock => mock.FindByIdAsync(cognitoUser.UserID))
                .ReturnsAsync(cognitoUser);

            // Act
            var result = await _classUnderTest.GetUserPublicProfile(new Guid(cognitoUser.UserID));

            // Assert
            Assert.IsType<UsersProfile>(result);
            _userManagerMock.Verify(mock => mock.FindByIdAsync(It.IsAny<string>()), Times.Once);

            Assert.Equal(cognitoUser.Attributes["email"], result.Email);
            Assert.Equal($"{cognitoUser.Attributes["name"]} {cognitoUser.Attributes["family_name"]}", result.Fullname);
            Assert.IsType<Guid>(result.UserId);
            Assert.Equal(cognitoUser.UserID, result.UserId.ToString());
        }
    }
}