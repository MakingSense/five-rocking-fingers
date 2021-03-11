using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace FRF.Web.Authorization
{
    public class ArtifactOwnershipPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public ArtifactOwnershipPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }

            switch (policyName)
            {
                case ArtifactAuthorization.Ownership:
                {
                    var policy = new AuthorizationPolicyBuilder();
                    policy.AddRequirements(new ArtifactOwnershipRequirement());
                    return Task.FromResult(policy.Build());
                }
                case ArtifactAuthorization.RelationsListOwnership:
                {
                    var policy = new AuthorizationPolicyBuilder();
                    policy.AddRequirements(new ArtifactsListOwnershipRequirement());
                    return Task.FromResult(policy.Build());
                }
                default:
                    return FallbackPolicyProvider.GetPolicyAsync(policyName);
            }
        }
    }
}