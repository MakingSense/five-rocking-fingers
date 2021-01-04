using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class SignInServiceTest : CognitoTestBase
    {
        private readonly SignInService _classUnderTest;

        public SignInServiceTest()
        {
            _classUnderTest = new SignInService(_signInManagerMock.Object);
        }

        [Fact]
        public async Task SignInAsync_WhenEmailNotExist_ReturnFalse()
        {
            // Arrange
            var userSignin = CreateUserSignIn();
            CognitoUser cognitoUser = null;
            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);

            // Act
            var result = await _classUnderTest.SignInAsync(userSignin);

            // Assert
            Assert.IsType<Tuple<bool, string>>(result);
            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SignInAsync_WhenPasswordIsNotValid_ReturnFalse()
        {
            // Arrange
            var userSignin = CreateUserSignIn();
            var cognitoUser = CreateCognitoUser();
            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);
            _signInManagerMock.Setup(mock =>
                    mock.PasswordSignInAsync(cognitoUser, userSignin.Password, userSignin.RememberMe, false))
                .ReturnsAsync(new SignInResult());

            // Act
            var result = await _classUnderTest.SignInAsync(userSignin);

            // Assert
            Assert.IsType<Tuple<bool, string>>(result);
            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Once);
        }

        [Fact]
        public async Task SignInAsync_ReturnTrueAndUserProfile()
        {
            // Arrange
            var userSignin = CreateUserSignIn();
            var cognitoUser = CreateCognitoUser();
            var signInResult = SignInResult.Success;

            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);
            _signInManagerMock.Setup(mock =>
                    mock.PasswordSignInAsync(cognitoUser, userSignin.Password, userSignin.RememberMe, false))
                .ReturnsAsync(signInResult);

            // Act
            var result = await _classUnderTest.SignInAsync(userSignin);

            // Assert
            Assert.IsType<Tuple<bool, string>>(result);
            Assert.True(result.Item1);
            Assert.Equal(cognitoUser.UserID, result.Item2);

            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Once);
        }
    }
}