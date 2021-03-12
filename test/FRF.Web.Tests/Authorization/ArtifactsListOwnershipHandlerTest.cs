using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using FRF.Core.Tests;
using FRF.DataAccess;
using FRF.DataAccess.EntityModels;
using FRF.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace FRF.Web.Tests.Authorization
{
    public class ArtifactsListOwnershipHandlerTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly DataAccessContextForTest _dataAccess;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly ArtifactsListOwnershipHandler _classUnderTest;
        private readonly Mock<IServiceProvider> _serviceProvider;

        public ArtifactsListOwnershipHandlerTest()
        {
            _configuration = new Mock<IConfiguration>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _serviceProvider = new Mock<IServiceProvider>();
            _classUnderTest = new ArtifactsListOwnershipHandler(_serviceProvider.Object, _httpContextAccessor.Object);
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
            var artifact2 = CreateArtifact(project);
            var relation = CreateArtifactsRelationModel(artifact.Id, artifact2.Id);
            var context = GetAuthorizationHandlerContext(currentUserId);
            var data = $"[{{\"artifact1Id\":{artifact.Id},\"artifact2Id\":{artifact2.Id},\"artifact1Property\":\"{relation.Artifact1Property}\",\"artifact2Property\":\"{relation.Artifact2Property}\",\"relationTypeId\":0}}]";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", artifact.Id.ToString() }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            httpContext.Request.Body = stream; 
            _httpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);
            MockServiceProvider();

            // Act.
            await _classUnderTest.HandleAsync(context);

            // Assert.
            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fail_WhenAnyArtifactsRelationIsNotOfCurrentUser()
        {
            // Arrange.
            var project = CreateProject();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            CreateUserByProject(project, anotherUserId);
            var artifact = CreateArtifact(project);
            var artifact2 = CreateArtifact(project);
            var relation = CreateArtifactsRelationModel(artifact.Id, artifact2.Id);
            var context = GetAuthorizationHandlerContext(currentUserId);
            var data = $"[{{\"artifact1Id\":{artifact.Id},\"artifact2Id\":{artifact2.Id},\"artifact1Property\":\"{relation.Artifact1Property}\",\"artifact2Property\":\"{relation.Artifact2Property}\",\"relationTypeId\":0}}]";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", artifact.Id.ToString() }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            httpContext.Request.Body = stream;
            _httpContextAccessor.SetupGet(accessor => accessor.HttpContext).Returns(httpContext);
            MockServiceProvider();

            // Act.
            await _classUnderTest.HandleAsync(context);

            // Assert.
            Assert.False(context.HasSucceeded);
        }

        [Fact]
        public async Task HandleRequirementAsync_Fail_WhenListOfRelationsIsEmpty()
        {
            // Arrange.
            var project = CreateProject();
            var currentUserId = Guid.NewGuid();
            CreateUserByProject(project, currentUserId);
            var context = GetAuthorizationHandlerContext(currentUserId);
            var data = "";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = _serviceProvider.Object
            };
            var routeArtifact = new Dictionary<string, string>
            {
                { "artifactId", int.MaxValue.ToString() }
            };
            httpContext.Request.RouteValues = new RouteValueDictionary(routeArtifact);
            httpContext.Request.Body = stream;
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
                new ArtifactsListOwnershipRequirement()
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
        private FRF.Core.Models.ArtifactsRelation CreateArtifactsRelationModel(int artifact1Id, int artifact2Id)
        {
            var random = new Random();
            var propertyId = random.Next(1000);
            var artifactRelation = new FRF.Core.Models.ArtifactsRelation()
            {
                Artifact1Id = artifact1Id,
                Artifact2Id = artifact2Id,
                Artifact1Property = "Mock 1 Property " + propertyId,
                Artifact2Property = "Mock 2 Property " + propertyId,
                RelationTypeId = 1
            };

            return artifactRelation;
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