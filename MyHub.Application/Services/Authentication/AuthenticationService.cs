using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Users.UsersDto;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;
		private readonly IPasswordEncryptionService _passwordEncryptionService;
		private readonly IMapper _mapper;

		public AuthenticationService(IConfiguration configuration, IUserService userService, IPasswordEncryptionService passwordEncryptionService, IMapper mapper)
		{
			_configuration = configuration;
			_userService = userService;
			_passwordEncryptionService = passwordEncryptionService;
			_mapper = mapper;
		}

		public bool RegisterUser(User user)
		{
			if (_userService.UserExists(user.Email))
				return false;

			var hashedPassword = _passwordEncryptionService.HashPassword(user.Password, out var salt);

			user.Id = Guid.NewGuid().ToString();
			user.Password = hashedPassword;
			user.Salt = Convert.ToHexString(salt);

			return _userService.RegisterUser(user) is not null;
		}

		public LoginDetails? AuthenticateUser(string email, string password)
		{
			var authenticatingUser = _userService.GetFullUserByEmail(email);

			if (authenticatingUser is null)
				return null;

			if (!_passwordEncryptionService.VerifyPassword(password, authenticatingUser.Password, authenticatingUser.Salt))
				return null;

			var tokens = GenerateAccessTokens(authenticatingUser);

			_userService.UpdateRefreshToken(authenticatingUser, tokens.RefreshToken);

			return SetLoginDetails(tokens, authenticatingUser);
		}

		public LoginDetails? RefreshUserAuthentication(string accessToken, string refreshToken)
		{
			var principle = GetPrincipleFromToken(accessToken);
			var userId = principle.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var user = _userService.GetFullUserById(userId);

			if (user is null)
				return null;

			if (user.RefreshToken != refreshToken)
			{
				_userService.RevokeUser(user);

				throw new InvalidOperationException("Login has been invalidated.");
			}

			var newAccessTokens = GenerateAccessTokens(user);

			_userService.UpdateRefreshToken(user, refreshToken);

			return SetLoginDetails(newAccessTokens, user);
		}

		private LoginDetails SetLoginDetails(Tokens tokens, User user)
			=> new() { Tokens = tokens, HubUserDto = _mapper.Map<HubUserDto>(user) };

		public bool RevokeUser(string userId) => _userService.RevokeUser(userId) is not null;

		private Tokens GenerateAccessTokens(User user)
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
					new Claim(JwtRegisteredClaimNames.Email, user.Email),
					new Claim(JwtRegisteredClaimNames.Name, user.Username),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddMinutes(15),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new Tokens { Token = tokenHandler.WriteToken(token), RefreshToken = Guid.NewGuid().ToString() };
		}

		private ClaimsPrincipal GetPrincipleFromToken(string token)
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
