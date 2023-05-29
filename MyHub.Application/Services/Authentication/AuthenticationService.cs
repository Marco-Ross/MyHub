using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Users.UsersDto;
using MyHub.Domain.Validation;
using MyHub.Domain.Validation.FluentValidators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly DomainOptions _domainOptions;
		private readonly AuthenticationOptions _authOptions;
		private readonly IUsersService _userService;
		private readonly IEncryptionService _encryptionService;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
		private readonly IValidator<UserRegisterValidator> _registerValidator;

		public AuthenticationService(IOptions<DomainOptions> domainOptions, IOptions<AuthenticationOptions> authOptions, IUsersService userService, IEncryptionService passwordEncryptionService, IMapper mapper, IEmailService emailService, IValidator<UserRegisterValidator> registerValidator)
		{
			_authOptions = authOptions.Value;
			_domainOptions = domainOptions.Value;
			_userService = userService;
			_encryptionService = passwordEncryptionService;
			_mapper = mapper;
			_emailService = emailService;
			_registerValidator = registerValidator;
		}

		public async Task<Validator> RegisterUser(AccessingUser accessingUser)
		{
			var validation = _registerValidator.Validate(new UserRegisterValidator(accessingUser));

			if (!validation.IsValid)
				return new Validator().AddError(string.Join(",", validation.Errors));

			var registerToken = _encryptionService.GenerateSecureToken();

			var registeredUser = _userService.RegisterUserDetails(accessingUser, registerToken);

			await _userService.UploadUserProfileImage(registeredUser);

			await _emailService.CreateAndSendEmail(new AccountRegisterEmail
			{
				UserId = registeredUser.Id,
				To = registeredUser.Email,
				ToName = registeredUser.User.Username,
				Subject = "Account Registration",
				RegisterToken = registerToken,
				ClientDomainAddress = _domainOptions.Client
			});

			return new Validator();
		}

		public async Task DeleteUser(string userId) => await _userService.DeleteUser(userId);

		public Validator VerifyUserEmail(string userId, string token)
		{
			var user = _userService.GetFullAccessingUserById(userId);
			if (user == null) return new Validator().AddError("Invalid user Id.");
			if (string.IsNullOrWhiteSpace(token)) return new Validator().AddError("Invalid user token.");

			return _userService.VerifyUserRegistration(user, token);
		}

		public Validator ResetPasswordLoggedIn(string userId, string oldPassword, string newPassword, string refreshToken)
		{
			var user = _userService.GetFullAccessingUserById(userId);

			if (user is null)
				return new Validator().AddError("User does not exist.");

			if (!_encryptionService.VerifyData(oldPassword, user.Password, user.PasswordSalt))
				return new Validator().AddError("Old password is invalid.");

			_userService.RevokeUserLoginsExceptCurrent(user, refreshToken);

			_userService.ResetUserPasswordLoggedIn(user, newPassword);

			return new Validator();
		}

		public async Task<Validator> ResetPasswordEmail(string email)
		{
			var user = _userService.GetFullAccessingUserByEmail(email);

			if (user is null)
				return new Validator().AddError("Email address does not exist.");

			if (!user.IsEmailVerified)
				return new Validator().AddError("Email address not verified.");

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
				ClientDomainAddress = _domainOptions.Client
			});

			return new Validator();
		}

		public Validator ResetPassword(string userId, string password, string resetPasswordToken)
		{
			var user = _userService.GetFullAccessingUserById(userId);
			if (user is null) return new Validator().AddError("Invalid user Id.");
			if (string.IsNullOrWhiteSpace(password)) return new Validator().AddError("Invalid password.");
			if (string.IsNullOrWhiteSpace(resetPasswordToken)) return new Validator().AddError("Invalid token.");

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

			_userService.AddRefreshToken(authenticatingUser, tokens.RefreshToken);

			return new Validator<LoginDetails>().Response(SetLoginDetails(tokens, authenticatingUser));
		}

		public string AuthenticateUserGetTokens(string userid, string email, string password)
		{
			var authenticatingUser = _userService.GetFullAccessingUserByEmail(email);

			if (authenticatingUser is null || authenticatingUser?.Id != userid)
				return string.Empty;

			if (!_encryptionService.VerifyData(password, authenticatingUser.Password, authenticatingUser.PasswordSalt))
				return string.Empty;

			return GenerateAccessTokens(authenticatingUser).AccessToken;
		}

		public Validator<LoginDetails> RefreshUserAuthentication(string accessToken, string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(accessToken))
				return new Validator<LoginDetails>().AddError("Access Token is invalid.");

			if (string.IsNullOrWhiteSpace(refreshToken))
				return new Validator<LoginDetails>().AddError("Refresh Token is invalid.");

			var principle = GetPrincipleFromToken(accessToken);

			if (principle is null)
				return new Validator<LoginDetails>().AddError("Token has been invalidated.");

			var userId = principle.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

			var user = _userService.GetFullAccessingUserById(userId);

			if (user is null)
				return new Validator<LoginDetails>().AddError("Invalid user Id.");

			if (!user.RefreshTokens.Any(x => x.Token == refreshToken))
			{
				_userService.RevokeUser(user, refreshToken);

				return new Validator<LoginDetails>().AddError("Login has been invalidated.");
			}

			var newAccessTokens = GenerateAccessTokens(user);

			_userService.UpdateRefreshToken(user, refreshToken, newAccessTokens.RefreshToken);

			return new Validator<LoginDetails>().Response(SetLoginDetails(newAccessTokens, user));
		}

		private LoginDetails SetLoginDetails(Tokens tokens, AccessingUser user)
			=> new() { Tokens = tokens, HubUserDto = _mapper.Map<HubUserDto>(user) };

		public bool RevokeUser(string userId, string refreshToken) => _userService.RevokeUser(userId, refreshToken) is not null;

		private Tokens GenerateAccessTokens(AccessingUser user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(_authOptions.JWT.Key);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _authOptions.JWT.Audience,
				Issuer = _authOptions.JWT.Issuer,
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

			return new Tokens { AccessToken = tokenHandler.WriteToken(token), RefreshToken = _encryptionService.GenerateSecureToken() };
		}

		private ClaimsPrincipal? GetPrincipleFromToken(string token)
		{
			var tokenValidationParameters = GetValidationParametersNoLifetime();

			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

				if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
					throw new SecurityTokenException("Invalid token.");

				return principal;
			}
			catch
			{
				return null;
			}
		}

		private TokenValidationParameters GetValidationParametersWithLifetime()
		{
			var validationParams = GetValidationParameters();

			validationParams.ValidateLifetime = true;

			return validationParams;
		}

		private TokenValidationParameters GetValidationParametersNoLifetime()
		{
			var validationParams = GetValidationParameters();

			validationParams.ValidateLifetime = false;

			return validationParams;
		}

		private TokenValidationParameters GetValidationParameters()
		{
			var tokenKey = Encoding.UTF8.GetBytes(_authOptions.JWT.Key);

			return new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				ValidAudience = _authOptions.JWT.Audience,
				ValidIssuer = _authOptions.JWT.Issuer,
				IssuerSigningKey = new SymmetricSecurityKey(tokenKey)
			};
		}

		private bool ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = GetValidationParametersWithLifetime();
			try
			{
				var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<Validator> ChangeUserEmail(string userId, string newEmail, string accessToken)
		{
			var validToken = ValidateToken(accessToken);

			if (!validToken)
				return new Validator().AddError("Invalid change email attempt.");

			if (_userService.UserExists(newEmail))
				return new Validator().AddError("Email already exists.");

			var user = _userService.GetFullAccessingUserById(userId);

			if (user is null)
				return new Validator().AddError("User does not exist.");

			if (user.ChangeEmailTokenExpireDate.HasValue && user.ChangeEmailTokenExpireDate > DateTime.Now)
				return new Validator().AddError("A valid reset email link has already been sent.");

			var changeEmailToken = _encryptionService.GenerateSecureToken();

			_userService.UpdateUserEmail(user, newEmail, changeEmailToken);

			await _emailService.CreateAndSendEmail(new EmailChangeEmail
			{
				UserId = userId,
				To = newEmail,
				ToName = user.User.Username,
				Subject = "Email Change Request",
				PreviousEmail = user.Email,
				ChangeEmailToken = changeEmailToken,
				ClientDomainAddress = _domainOptions.Client
			});

			return new Validator();
		}

		public Validator ChangeUserEmailComplete(string userId, string changeEmailToken)
		{
			var user = _userService.GetFullAccessingUserById(userId);
			if (user is null) return new Validator().AddError("Invalid user Id.");
			if (string.IsNullOrWhiteSpace(changeEmailToken)) return new Validator().AddError("Invalid token.");

			_userService.RevokeAllUserLogins(user);

			return _userService.ChangeUserEmailComplete(user, changeEmailToken);
		}
	}
}
