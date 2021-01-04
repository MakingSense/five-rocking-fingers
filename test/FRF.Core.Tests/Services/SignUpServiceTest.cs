using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FRF.Core.Tests.Services
{
    public class SignUpServiceTest : CognitoTestBase
    {
        private readonly SignUpService _classUnderTest;
        private readonly Mock<CognitoUserPool> _cognitoUserPool;

        public SignUpServiceTest()
        {
            _cognitoUserPool =
                new Mock<CognitoUserPool>("region_poolName", "clientID", _cognitoClientMock.Object, null);
            _classUnderTest = new SignUpService(
                _signInManagerMock.Object,
                _cognitoClientMock.Object,
                _userManagerMock.Object,
                _cognitoUserPool.Object);
        }

        [Fact]
        public async Task SignUpAsync_WhenSignUpFail_ReturnFalse()
        {
            // Arrange
            var newUser = CreateUser();
            var cognitoUser = CreateCognitoUser();

            _userManagerMock
                .Setup(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _classUnderTest.SignUpAsync(newUser);

            // Assert
            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item2);
            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Never);
        }

        [Fact]
        public async Task SignUpAsync_WhenAutoVerificationFail_ReturnFalse()
        {
            // Arrange
            var newUser = CreateUser();
            var cognitoUser = CreateCognitoUser();

            _userManagerMock
                .Setup(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _cognitoClientMock
                .Setup(mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AdminUpdateUserAttributesResponse());
            // Act
            var result = await _classUnderTest.SignUpAsync(newUser);

            // Assert
            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()), Times.Once);
            _cognitoClientMock.Verify(
                mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Never);
        }

        [Fact]
        public async Task SignUpAsync_WhenAutoConfirmSignUpFail_ReturnFalse()
        {
            // Arrange
            var newUser = CreateUser();
            var cognitoUser = CreateCognitoUser();

            _userManagerMock
                .Setup(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _cognitoClientMock
                .Setup(mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AdminUpdateUserAttributesResponse());

            _cognitoClientMock
                .Setup(mock =>
                    mock.AdminConfirmSignUpAsync(It.IsAny<AdminConfirmSignUpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AdminConfirmSignUpResponse());

            // Act
            var result = await _classUnderTest.SignUpAsync(newUser);

            // Assert
            Assert.False(result.Item1);
            Assert.Equal(string.Empty, result.Item2);

            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()), Times.Once);
            _cognitoClientMock.Verify(
                mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            _cognitoClientMock.Verify(
                mock => mock.AdminConfirmSignUpAsync(It.IsAny<AdminConfirmSignUpRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Never);
        }

        [Fact]
        public async Task SignUpAsync_ReturnTrue()
        {
            // Arrange
            var newUser = CreateUser();
            var cognitoUser = CreateCognitoUser();
            var userSignin = CreateUserSignIn();
            var signInResult = SignInResult.Success;

            _userManagerMock
                .Setup(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _cognitoClientMock
                .Setup(mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AdminUpdateUserAttributesResponse());

            _cognitoClientMock
                .Setup(mock =>
                    mock.AdminConfirmSignUpAsync(It.IsAny<AdminConfirmSignUpRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AdminConfirmSignUpResponse());

            _userManagerMock
                .Setup(mock => mock.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(cognitoUser);

            _signInManagerMock.Setup(mock =>
                    mock.PasswordSignInAsync(cognitoUser, userSignin.Password, userSignin.RememberMe, false))
                .ReturnsAsync(signInResult);

            // Act
            var result = await _classUnderTest.SignUpAsync(newUser);

            // Assert
            Assert.IsType<Tuple<bool, string>>(result);
            Assert.True(result.Item1);
            Assert.Equal(cognitoUser.UserID, result.Item2);

            _userManagerMock.Verify(mock => mock.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            _signInManagerMock.Verify(
                mock => mock.PasswordSignInAsync(It.IsAny<CognitoUser>(), It.IsAny<string>(), false, false),
                Times.Once);
            _userManagerMock.Verify(mock => mock.CreateAsync(It.IsAny<CognitoUser>(), It.IsAny<string>()), Times.Once);
            _cognitoClientMock.Verify(
                mock => mock.AdminUpdateUserAttributesAsync(It.IsAny<AdminUpdateUserAttributesRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once);
            _cognitoClientMock.Verify(
                mock => mock.AdminConfirmSignUpAsync(It.IsAny<AdminConfirmSignUpRequest>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}