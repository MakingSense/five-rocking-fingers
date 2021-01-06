using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using FRF.Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace FRF.Core.Tests.Services
{
    public class CognitoTestBase
    {
        protected Mock<SignInManager<CognitoUser>> _signInManagerMock;
        protected Mock<UserManager<CognitoUser>> _userManagerMock;
        protected Mock<IAmazonCognitoIdentityProvider> _cognitoClientMock;
        protected Mock<CognitoUserPool> _cognitoPoolMock;

        public CognitoTestBase()
        {
            _cognitoClientMock = new Mock<IAmazonCognitoIdentityProvider>();
            _cognitoPoolMock =
                new Mock<CognitoUserPool>("region_poolName", "clientID", _cognitoClientMock.Object, null);
            _userManagerMock = new Mock<UserManager<CognitoUser>>(
                Mock.Of<IUserStore<CognitoUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<CognitoUser>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<CognitoUser>>(), null, null, null,
                null);
        }

        public static DefaultHttpContext MockContext(CognitoUser cognitoUser)
        {
            var context = new DefaultHttpContext();
            var authMock = new Mock<IAuthenticationService>();
            var userPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Name, cognitoUser.UserID),
                new Claim("providerKey", "login"),
                new Claim("sub", new Guid().ToString()),
                new Claim(ClaimTypes.AuthenticationMethod, "providerKey")
            }));
            context.User = userPrincipal;

            context.RequestServices = new ServiceCollection().AddSingleton(authMock.Object).BuildServiceProvider();

            return context;
        }

        public CognitoUser CreateCognitoUser()
        {
            var userId = Guid.NewGuid();
            return new CognitoUser(userId.ToString(), "clientId", _cognitoPoolMock.Object,
                _cognitoClientMock.Object)
            {
                Attributes =
                {
                    ["family_name"] = "lastName",
                    ["name"] = "name",
                    ["email"] = "mock@email.com"
                }
            };
        }

        public static UserSignIn CreateUserSignIn()
        {
            return new UserSignIn
            {
                Email = "mock@email.com",
                Password = "Mockpasword123",
                RememberMe = false
            };
        }

        public static User CreateUser()
        {
            return new User
            {
                Email = "mock@email.com",
                FamilyName = "familyName",
                Name = "name",
                Password = "Mockpasword123"
            };
        }
    }
}