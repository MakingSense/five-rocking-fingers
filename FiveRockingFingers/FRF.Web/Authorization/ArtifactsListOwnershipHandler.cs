using FRF.DataAccess;
using FRF.Web.Dtos.Artifacts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Web.Authorization
{
    public class ArtifactsListOwnershipHandler : AuthorizationHandler<ArtifactsListOwnershipRequirement>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ArtifactsListOwnershipHandler(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ArtifactsListOwnershipRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var artifactsRelations = await GetArtifactsRelationInRequestBody();
            if (userId == null || artifactsRelations==null )
            {
                return;
            }

            var areAllArtifactsRelationOfCurrentUser = await AreAllArtifactsRelationOfCurrentUser(artifactsRelations, userId);
            if (!areAllArtifactsRelationOfCurrentUser)
            {
                return;
            }

            context.Succeed(requirement);
            return;
        }

        private async Task<IList<ArtifactsRelationInsertDTO>> GetArtifactsRelationInRequestBody()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, false, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            var artifactsRelations = JsonConvert.DeserializeObject<List<ArtifactsRelationInsertDTO>>(requestBody);
            request.Body.Position = 0;
            return artifactsRelations;
        }

        private async Task<bool> AreAllArtifactsRelationOfCurrentUser(IList<ArtifactsRelationInsertDTO> artifactsRelations, string userId)
        {
            var isAuthorized = false;
            using var scope = _serviceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetRequiredService<DataAccessContext>();
            var artifactsIds = artifactsRelations
                .Select(ar => ar.Artifact1Id)
                .Concat(artifactsRelations.Select(ar => ar.Artifact2Id));
            var artifactsByUser = await dataContext.Artifacts
                .Include(artifact =>
                    artifact.Project).Where(artifact => artifact.Project.UsersByProject
                    .Any(ubp => ubp.UserId == Guid.Parse(userId))).ToListAsync();

            isAuthorized = artifactsIds.All(aid =>
                artifactsByUser.Any(abu => abu.Id == aid));

            return isAuthorized;
        }
    }
}