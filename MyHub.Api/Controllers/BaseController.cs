﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace MyHub.Api.Controllers
{
	public class BaseController : ControllerBase
	{
		protected string UserId => User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? string.Empty;
		protected string LoginIssuer => User.FindFirst(JwtRegisteredClaimNames.Iss)?.Value ?? string.Empty;
	}
}
