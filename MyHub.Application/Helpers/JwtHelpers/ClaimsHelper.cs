using MyHub.Domain.Authentication.Claims;
using MyHub.Domain.Enums.Enumerations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyHub.Application.Helpers.JwtHelpers
{
	public static class ClaimsHelper
	{
		public static Claim[] CreateClaims(HubClaims claims)
		{
			return new Claim[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, claims.Sub ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.FamilyName, claims.FamilyName ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.GivenName, claims.GivenName ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.Email, claims.Email ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.Name, claims.Name ?? string.Empty),
				new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iss, claims.Iss),
				new Claim(CustomJwtClaimNames.IssuerManaging.Id, claims.IssManaging)
			};
		}

		public static string GetClaim(JwtSecurityToken securityToken, string claim)
		{
			return securityToken.Claims.FirstOrDefault(x => x.Type == claim)?.Value ?? string.Empty;
		}
	}
}
