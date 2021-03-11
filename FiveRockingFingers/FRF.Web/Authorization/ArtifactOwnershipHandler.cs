using FRF.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FRF.Web.Authorization
{
    public class ArtifactOwnershipHandler : AuthorizationHandler<ArtifactOwnershipRequirement>
    {
        private const string ArtifactIdParameter = "artifactId";
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArtifactOwnershipHandler(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ArtifactOwnershipRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var artifactId = GetArtifactIdFromRequest();

            if (userId == null || !artifactId.HasValue || artifactId <= 0)
            {
                return Task.CompletedTask;
            }

            if (!IsArtifactOfCurrentUser(artifactId, userId))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        private int? GetArtifactIdFromRequest()
        {
            var request = _httpContextAccessor.HttpContext.Request;

            if (!request.RouteValues.TryGetValue(ArtifactIdParameter, out var id)) return null;
            
            if (!int.TryParse(id.ToString(), out var artifactId)) return null;

            return artifactId;
        }
        private bool IsArtifactOfCurrentUser(int? artifactId, string userId)
        {
            var isArtifactOfCurrentUser = false;

            using var scope = _serviceProvider.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            if (dataContext.Artifacts.Any(a => a.Id == artifactId))
            {
                isArtifactOfCurrentUser = dataContext.Artifacts.Include(artifact => artifact.Project).Any(ap =>
                    ap.Project.UsersByProject.Any(ubp => ubp.UserId.Equals(Guid.Parse(userId))) &&
                    ap.Id == artifactId);
            }

            return isArtifactOfCurrentUser;
        }

    }
}