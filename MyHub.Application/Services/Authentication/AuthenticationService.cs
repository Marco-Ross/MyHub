using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;

		public AuthenticationService(IConfiguration configuration, IUserService userService)
		{
			_configuration = configuration;
			_userService = userService;
		}

		public Tokens AuthenticateUser(string username, string password)
		{
			var authenticatingUser = _userService.GetUserWithCredentials(username, password);

			if (authenticatingUser is null)
				return null;

			var tokens = GenerateAccessTokens(authenticatingUser);

			if (tokens is null)
				return null;

			SetUserRefreshToken(authenticatingUser, tokens.RefreshToken);

			return tokens;
		}
		public Tokens RefreshUserAuthentication(string accessToken, string refreshToken)
		{
			var principle = GetPrincipleFromExpiredToken(accessToken);
			var userId = principle.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var user = _userService.GetUser(userId);

			if (user.RefreshToken != refreshToken /*|| user.RefreshTokenExpiryTime <= DateTime.Now*/)
			{
				user.RefreshToken = null;
				//revoke all user tokens
				_userService.UpdateUser(user);

				throw new InvalidOperationException("Login has be invalidated.");
			}

			var newAccessTokens = GenerateAccessTokens(user);

			SetUserRefreshToken(user, newAccessTokens.RefreshToken);

			return newAccessTokens;
		}

		private void SetUserRefreshToken(User user, string refreshToken)
		{
			user.RefreshToken = refreshToken;

			_userService.UpdateUser(user);
		}

		public Tokens GenerateAccessTokens(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? string.Empty);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _configuration["JWT:Audience"],
				Issuer = _configuration["JWT:Issuer"],
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(JwtRegisteredClaimNames.Sub, user.Id),
					new Claim(JwtRegisteredClaimNames.Name, user.Username),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddSeconds(15), //15min
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new Tokens { Token = tokenHandler.WriteToken(token), RefreshToken = Guid.NewGuid().ToString() };
		}

		private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
		{
			var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? string.Empty);

			var tokenValidationParameters = new TokenValidationParameters
			{				
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = false,
				ValidAudience = _configuration["JWT:Audience"],
				ValidIssuer = _configuration["JWT:Issuer"],
				IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");

			return principal;
		}
	}
}
