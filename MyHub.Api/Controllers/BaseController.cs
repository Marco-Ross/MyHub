using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Domain.Enums.Enumerations;

namespace MyHub.Api.Controllers
{
	public class BaseController : ControllerBase
	{
		protected string UserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
		protected LoginIssuers IssuerManaging => Enumeration.FromId<LoginIssuers>(User.FindFirst(CustomJwtClaimNames.IssuerManaging.Id)?.Value ?? string.Empty) ?? LoginIssuers.MarcosHub;
	}
}
