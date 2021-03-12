using FRF.DataAccess;
using FRF.DataAccess.EntityModels;
using FRF.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace FRF.Web.Tests.Authorization
{
    public class ArtifactOwnershipHandlerTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly DataAccessContextForTest _dataAccess;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly ArtifactOwnershipHandler _classUnderTest;
        private readonly Mock<IServiceProvider> _serviceProvider;

        public ArtifactOwnershipHandlerTest()
        {
            _configuration = new Mock<IConfiguration>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _serviceProvider = new Mock<IServiceProvider>();
            _classUnderTest = new ArtifactOwnershipHandler(_serviceProvider.Object, _httpContextAccessor.Object);
            _dataAccess = new DataAccessContextForTest(Guid.NewGuid(), _configuration.Object);
            _dataAccess.Database.EnsureCreated();
        }

        [Fact]
        public async Task HandleRequirementAsync_Succeeds()
        {
            // Arrange.
            var project = CreateProject();
            var currentUserId = Guid.NewGuid();
            CreateUserByProject(project, currentUserId);
            var artifact = CreateArtifact(project);
            var context = GetAuthorizationHandlerContext(currentUserId);
            
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", artifact.Id.ToString() }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            _httpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);
            MockServiceProvider();

            // Act.
            await _classUnderTest.HandleAsync(context);

            // Assert.
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fail_WhenArtifactIsNotOfCurrentUser()
        {
            // Arrange.
            var project = CreateProject();
            var artifact = CreateArtifact(project);
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var context = GetAuthorizationHandlerContext(currentUserId);
            CreateUserByProject(project, anotherUserId);

            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", artifact.Id.ToString() }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            _httpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);
            MockServiceProvider();

            // Act.
            await _classUnderTest.HandleAsync(context);

            // Assert.
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fail_WhenArtifactIdNotExist()
        {
            // Arrange.
            var project = CreateProject();
            var currentUserId = Guid.NewGuid();
            CreateUserByProject(project, currentUserId);
            var context = GetAuthorizationHandlerContext(currentUserId); ;

            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", "9999" }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            _httpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);
            MockServiceProvider();

            // Act.
            await _classUnderTest.HandleAsync(context);

            // Assert.
            Assert.False(context.HasSucceeded);
        }

        private AuthorizationHandlerContext GetAuthorizationHandlerContext(Guid currentUserId)
        {
            var userPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString()),
            }));
            var requirements = new List<IAuthorizationRequirement>
            {
                new ArtifactOwnershipRequirement()
            };
            var context = new AuthorizationHandlerContext(requirements, userPrincipal, null);
            return context;
        }

        private void MockServiceProvider()
        {
            _serviceProvider
                .Setup(x => x.GetService(typeof(DataAccessContext)))
                .Returns(_dataAccess);

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            _serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);
        }

        private void CreateUserByProject(DataAccess.EntityModels.Project project, Guid userId)
        {
            var userByProject = new UsersByProject
            {
                ProjectId = project.Id,
                Project = project,
                UserId = userId
            };

            _dataAccess.UsersByProject.Add(userByProject);
            _dataAccess.SaveChanges();
        }

        private Provider CreateProvider()
        {
            var provider = new Provider();
            provider.Name = "[Mock] Provider name";
            _dataAccess.Providers.Add(provider);
            _dataAccess.SaveChanges();

            return provider;
        }

        private ArtifactType CreateArtifactType(Provider provider)
        {
            var artifactType = new ArtifactType();
            artifactType.Name = "[Mock] Artifact type name";
            artifactType.Description = "[Mock] Artifact type description";
            artifactType.Provider = provider;
            _dataAccess.ArtifactType.Add(artifactType);
            _dataAccess.SaveChanges();

            return artifactType;
        }

        private Project CreateProject()
        {
            var project = new Project();
            project.Name = "[MOCK] Project name";
            project.CreatedDate = DateTime.Now;
            project.ProjectCategories = new List<ProjectCategory>();
            _dataAccess.Projects.Add(project);
            _dataAccess.SaveChanges();

            return project;
        }

        private Artifact CreateArtifact(Project project)
        {
            var provider = CreateProvider();
            var artifactType = CreateArtifactType(provider);
            var random = new Random();
            var artifact = new Artifact()
            {
                Name = "[Mock] Artifact name " + random.Next(500),
                CreatedDate = DateTime.Now,
                Project = project,
                ProjectId = project.Id,
                ArtifactType = artifactType,
                ArtifactTypeId = artifactType.Id
            };
            _dataAccess.Artifacts.Add(artifact);
            _dataAccess.SaveChanges();

            return artifact;
        }
    }
}