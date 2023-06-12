using Microsoft.IdentityModel.JsonWebTokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Enums.Enumerations;
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
				new Claim(JwtRegisteredClaimNames.Iss, claims.Iss ?? string.Empty),
				new Claim(CustomJwtClaimNames.Picture.Id, claims.Pic ?? string.Empty),
			};
		}
	}
}
