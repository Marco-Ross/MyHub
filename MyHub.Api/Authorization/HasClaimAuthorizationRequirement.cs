using Microsoft.AspNetCore.Authorization;

namespace MyHub.Api.Authorization
{
    public class HasClaimAuthorizationRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public HasClaimAuthorizationRequirement(string claimType)
        {
            ClaimType = claimType;
        }
    }
}
