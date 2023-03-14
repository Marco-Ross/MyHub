using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Users.UsersDto;
using MyHub.Domain.Validation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;
		private readonly IEncryptionService _encryptionService;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;

		public AuthenticationService(IConfiguration configuration, IUserService userService, IEncryptionService passwordEncryptionService, IMapper mapper, IEmailService emailService)
		{
			_configuration = configuration;
			_userService = userService;
			_encryptionService = passwordEncryptionService;
			_mapper = mapper;
			_emailService = emailService;
		}

		public async Task<Validator> RegisterUser(string email, string username, string password)
		{
			if (_userService.UserExists(email))
				return new Validator().AddError("Email address already exists.");

			var registerToken = _encryptionService.GenerateSecureToken();

			var registeredUser = _userService.RegisterUser(email, username, password, registerToken);

			await _emailService.CreateAndSendEmail(new AccountRegisterEmail
			{
				UserId = registeredUser.Id,
				To = registeredUser.Email,
				ToName = registeredUser.User.Username,
				Subject = "Account Registration",
				RegisterToken = registerToken,
				ClientDomainAddress = _configuration["Domain:Client"] ?? string.Empty
			});

			return new Validator();
		}

		public Validator VerifyUserEmail(string userId, string token)
		{
			var user = _userService.GetFullAccessingUserById(userId);
			if (user == null) return new Validator().AddError("Invalid user Id.");

			return _userService.VerifyUserRegistration(user, token);
		}

		public async Task<Validator> ResetPasswordEmail(string email)
		{
			var user = _userService.GetFullAccessingUserByEmail(email);

			if (user is null) 
				return new Validator().AddError("Email address does not exist.");

			if (user.ResetPasswordTokenExpireDate.HasValue && user.ResetPasswordTokenExpireDate > DateTime.Now)
				return new Validator().AddError("A valid reset password link has already been sent.");

			var resetToken = _encryptionService.GenerateSecureToken();

			var resetUser = _userService.ResetUserPassword(user, resetToken);

			await _emailService.CreateAndSendEmail(new PasswordRecoveryEmail
			{
				UserId = resetUser.Id,
				To = resetUser.Email,
				ToName = resetUser.User.Username,
				Subject = "Password Recovery",
				ResetPasswordToken = resetToken,
				ClientDomainAddress = _configuration["Domain:Client"] ?? string.Empty
			});

			return new Validator();
		}

		public Validator ResetPassword(string userId, string password, string resetPasswordToken)
		{
			var user = _userService.GetFullAccessingUserById(userId);
			if (user is null) return new Validator().AddError("Invalid user Id.");

			return _userService.VerifyUserPasswordReset(user, password, resetPasswordToken);
		}

		public Validator<LoginDetails> AuthenticateUser(string email, string password)
		{
			var authenticatingUser = _userService.GetFullAccessingUserByEmail(email);

			if (authenticatingUser is null || !_encryptionService.VerifyData(password, authenticatingUser.Password, authenticatingUser.PasswordSalt))
				return new Validator<LoginDetails>().AddError("Invalid Login Credentials.");
			
			if (!authenticatingUser.IsEmailVerified)
				return new Validator<LoginDetails>().AddError("Email address not verified.");

			var tokens = GenerateAccessTokens(authenticatingUser);

			_userService.UpdateRefreshToken(authenticatingUser, tokens.RefreshToken);
			
			return new Validator<LoginDetails>().Response(SetLoginDetails(tokens, authenticatingUser));
		}

		public Validator<LoginDetails> RefreshUserAuthentication(string accessToken, string refreshToken)
		{
			var principle = GetPrincipleFromToken(accessToken);
			var userId = principle.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var user = _userService.GetFullAccessingUserById(userId);

			if (user is null)
				return new Validator<LoginDetails>().AddError("Invalid user Id.");

			if (user.RefreshToken != refreshToken)
			{
				_userService.RevokeUser(user);

				return new Validator<LoginDetails>().AddError("Login has been invalidated.");
			}

			var newAccessTokens = GenerateAccessTokens(user);

			_userService.UpdateRefreshToken(user, refreshToken);

			return new Validator<LoginDetails>().Response(SetLoginDetails(newAccessTokens, user));
		}

		private LoginDetails SetLoginDetails(Tokens tokens, AccessingUser user)
			=> new() { Tokens = tokens, HubUserDto = _mapper.Map<HubUserDto>(user) };

		public bool RevokeUser(string userId) => _userService.RevokeUser(userId) is not null;

		private Tokens GenerateAccessTokens(AccessingUser user)
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
					new Claim(JwtRegisteredClaimNames.Name, user.User.Username),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddMinutes(15),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new Tokens { Token = tokenHandler.WriteToken(token), RefreshToken = _encryptionService.GenerateSecureToken() };
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
