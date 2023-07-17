using Microsoft.AspNetCore.Authorization;

namespace MyHub.Api.Authorization
{
	public class HasClaimAuthorizationHandler : AuthorizationHandler<HasClaimAuthorizationRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasClaimAuthorizationRequirement requirement)
		{
			if (context.User.HasClaim(requirement.ClaimType, "true"))
				context.Succeed(requirement);	

			return Task.CompletedTask;
		}
	}
}
