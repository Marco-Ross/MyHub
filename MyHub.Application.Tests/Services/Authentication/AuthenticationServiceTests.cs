using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using MyHub.Application.Services.Authentication;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Domain.Enums.Enumerations;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation.FluentValidators;

namespace MyHub.Application.Tests.Services.Authentication
{
	public class AuthenticationServiceTests : ApplicationTestBase
	{
		private readonly IAuthenticationService _sut;

		private readonly IOptions<DomainOptions> _domainOptions = Options.Create(GetDomainOptions());
		private readonly IOptions<AuthenticationOptions> _authenticationOptions = Options.Create(GetAuthOptions());
		private readonly Mock<IUsersService> _userService = new();
		private readonly Mock<IEncryptionService> _encryptionService = new();
		private readonly Mock<IMapper> _mapper = new();
		private readonly Mock<IEmailService> _emailService = new();
		private readonly Mock<IValidator<UserRegisterValidator>> _registerValidator = new();

		private readonly AccessingUser USER;
		private const string ACCESSTOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJUZXN0VXNlcklkIiwiZW1haWwiOiJUZXN0QEVtYWlsLmNvbSIsIm5hbWUiOiJUZXN0VXNlciIsImlhdCI6MTY3OTMxNTc4MCwianRpIjoiZjBhMDIyYzctMWM5ZS00N2E0LWJlNDQtZjMxNTg4YjJiMTk1IiwibmJmIjoxNjc5MzE1NzgwLCJleHAiOjE2NzkzMTY2ODAsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0IiwiYXVkIjoibWFyY29zLWh1YiJ9.dUXE1BTW5BUPSZHLP8kEPn2OEsfxRAdnryBAlKbjtNA";

		public AuthenticationServiceTests()
		{
			USER = new AccessingUser { Id = "TestUserId", User = new User { Email = "Test@Email.com", Username = "TestUser" }, Password = "TestPassword", PasswordSalt = "" };
			_sut = new AuthenticationService(_domainOptions, _authenticationOptions, _userService.Object, _encryptionService.Object, _mapper.Object, _emailService.Object, _registerValidator.Object);
		}

		[Fact]
		public async Task RegisterUser_UserExists_ReturnsInvalid()
		{
			//Arrange
			_userService.Setup(x => x.UserExists(USER.User.Email)).Returns(true);

			_registerValidator.Setup(x => x.Validate(It.IsAny<UserRegisterValidator>())).Returns(GetValidationError("Email", "User exists."));

			//Act
			var validator = await _sut.RegisterUser(USER);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task RegisterUser_EnsureUserExistsAndUserSaves_ReturnsValid()
		{
			//Arrange
			_userService.Setup(x => x.UserExists(USER.User.Email)).Returns(true);
			_userService.Setup(x => x.RegisterUserDetails(It.IsAny<AccessingUser>(), It.IsAny<string>())).Returns(new AccessingUser());

			_registerValidator.Setup(x => x.Validate(It.IsAny<UserRegisterValidator>())).Returns(GetValidationResult);

			//Act
			var validator = await _sut.RegisterUser(USER);

			//Assert
			Assert.True(validator.IsValid);
			_userService.Verify(x => x.RegisterUserDetails(It.IsAny<AccessingUser>(), It.IsAny<string>()));
			_emailService.Verify(x => x.CreateAndSendEmail(It.IsAny<AccountRegisterEmail>()));
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void VerifyUserEmail_NullOrEmptyUserId_ReturnsInvalid(string userId)
		{
			//Arrange

			//Act
			var validator = _sut.VerifyUserEmail(userId, string.Empty);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void VerifyUserEmail_NullOrEmptyToken_ReturnsInvalid(string token)
		{
			//Arrange

			//Act
			var validator = _sut.VerifyUserEmail(string.Empty, token);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public async Task ResetPasswordEmail_NullOrEmptyEmail_ReturnsInvalid(string email)
		{
			//Arrange

			//Act
			var validator = await _sut.ResetPasswordEmail(email);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ResetPasswordEmail_EmailNotValidated_ReturnsInvalid()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);

			//Act
			var validator = await _sut.ResetPasswordEmail(USER.User.Email);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ResetPasswordEmail_HasActiveResetPasswordTokenExpireDate_ReturnsInvalid()
		{
			//Arrange
			USER.IsEmailVerified = true;
			USER.ResetPasswordTokenExpireDate = DateTime.Now.AddHours(1);
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);

			//Act
			var validator = await _sut.ResetPasswordEmail(USER.User.Email);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ResetPasswordEmail_HasExistingResetPasswordTokenDateAlreadyExpired_ReturnsValid()
		{
			//Arrange
			USER.IsEmailVerified = true;
			USER.ThirdPartyDetails.ThirdPartyIssuerId = LoginIssuers.MarcosHub.Id;
			USER.ResetPasswordTokenExpireDate = DateTime.Now.AddHours(-1);
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);

			_userService.Setup(x => x.ResetUserPassword(It.IsAny<AccessingUser>(), It.IsAny<string>())).Returns(USER);

			//Act
			var validator = await _sut.ResetPasswordEmail(USER.User.Email);

			//Assert
			Assert.True(validator.IsValid);
			_emailService.Verify(x => x.CreateAndSendEmail(It.IsAny<PasswordRecoveryEmail>()));
		}
		
		[Fact]
		public async Task ResetPasswordEmail_LoginIssuerNotLocal_ReturnsInvalid()
		{
			//Arrange
			USER.ThirdPartyDetails.ThirdPartyIssuerId = LoginIssuers.Google.Id;
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);

			//Act
			var validator = await _sut.ResetPasswordEmail(USER.User.Email);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ResetPasswordEmail_HasNoExistingResetPasswordTokenDate_ReturnsValid()
		{
			//Arrange
			USER.IsEmailVerified = true;
			USER.ThirdPartyDetails.ThirdPartyIssuerId = LoginIssuers.MarcosHub.Id;
			USER.ResetPasswordTokenExpireDate = null;
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);

			_userService.Setup(x => x.ResetUserPassword(It.IsAny<AccessingUser>(), It.IsAny<string>())).Returns(USER);

			//Act
			var validator = await _sut.ResetPasswordEmail(USER.User.Email);

			//Assert
			Assert.True(validator.IsValid);
			_emailService.Verify(x => x.CreateAndSendEmail(It.IsAny<PasswordRecoveryEmail>()));
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void ResetPassword_NullOrEmptyUserId_ReturnsInvalid(string userId)
		{
			//Arrange

			//Act
			var validator = _sut.ResetPassword(userId, string.Empty, string.Empty);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void ResetPassword_NullOrEmptyPassword_ReturnsInvalid(string password)
		{
			//Arrange

			//Act
			var validator = _sut.ResetPassword(string.Empty, password, string.Empty);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void ResetPassword_NullOrEmptyToken_ReturnsInvalid(string token)
		{
			//Arrange

			//Act
			var validator = _sut.ResetPassword(string.Empty, string.Empty, token);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void AuthenticateUser_NullOrEmptyEmail_ReturnsInvalid(string email)
		{
			//Arrange

			//Act
			var validator = _sut.AuthenticateUser(email, USER.Password);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void AuthenticateUser_NullOrEmptyPassword_ReturnsInvalid(string password)
		{
			//Arrange

			//Act
			var validator = _sut.AuthenticateUser(USER.User.Email, password);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void AuthenticateUser_IncorrectPassword_ReturnsInvalid()
		{
			//Arrange
			_encryptionService.Setup(x => x.VerifyData(USER.Password, "Pass", "PassSalt")).Returns(false);

			//Act
			var validator = _sut.AuthenticateUser(USER.User.Email, USER.Password);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void AuthenticateUser_ValidPasswordEmailNotVerified_ReturnsInvalid()
		{
			//Arrange
			_encryptionService.Setup(x => x.VerifyData(USER.Password, "Pass", "PassSalt")).Returns(true);
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(new AccessingUser());

			//Act
			var validator = _sut.AuthenticateUser(USER.User.Email, USER.Password);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void AuthenticateUser_ValidPasswordEmailVerified_ReturnsValid()
		{
			//Arrange
			USER.IsEmailVerified = true;
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var validator = _sut.AuthenticateUser(USER.User.Email, USER.Password);

			//Assert
			Assert.True(validator.IsValid);
			Assert.NotNull(validator.ResponseValue);
			_userService.Verify(x => x.AddRefreshToken(It.IsAny<AccessingUser>(), It.IsAny<string>()));
		}


		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void RefreshUserAuthentication_NullOrEmptyAccessToken_ReturnsInvalid(string accessToken)
		{
			//Arrange

			//Act
			var validator = _sut.RefreshUserAuthentication(accessToken, "RefreshToken");

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void RefreshUserAuthentication_NullOrEmptyRefreshToken_ReturnsInvalid(string refreshToken)
		{
			//Arrange

			//Act
			var validator = _sut.RefreshUserAuthentication("AccessToken", refreshToken);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void RefreshUserAuthentication_InvalidJWTToken_ReturnsInvalid()
		{
			//Arrange

			//Act
			var validator = _sut.RefreshUserAuthentication("InvalidToken", "RefreshToken");

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void RefreshUserAuthentication_ValidJWTTokenInvalidUser_ReturnsInvalid()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(() => null);

			//Act
			var validator = _sut.RefreshUserAuthentication(ACCESSTOKEN, "RefreshToken");

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public void RefreshUserAuthentication_InvalidRefreshToken_ReturnsInvalid()
		{
			//Arrange
			USER.RefreshTokens = new List<RefreshToken> { new RefreshToken { Token = "InvalidRefreshToken" } };
			_userService.Setup(x => x.GetFullAccessingUserById(USER.Id)).Returns(USER);

			//Act
			var validator = _sut.RefreshUserAuthentication(ACCESSTOKEN, "RefreshToken");

			//Assert
			Assert.True(validator.IsInvalid);
			_userService.Verify(x => x.RevokeUser(It.IsAny<AccessingUser>(), It.IsAny<string>()));
		}

		[Fact]
		public void RefreshUserAuthentication_ValidData_ReturnsValid()
		{
			//Arrange
			USER.RefreshTokens = new List<RefreshToken> { new RefreshToken { Token = "ValidRefreshToken" } };
			_userService.Setup(x => x.GetFullAccessingUserById(USER.Id)).Returns(USER);

			//Act
			var validator = _sut.RefreshUserAuthentication(ACCESSTOKEN, USER.RefreshTokens.First().Token);

			//Assert
			Assert.True(validator.IsValid);
			_userService.Verify(x => x.UpdateRefreshToken(It.IsAny<AccessingUser>(), It.IsAny<string>(), It.IsAny<string>()));
		}
		
		[Fact]
		public void ResetPasswordLoggedIn_InvalidUser_ReturnsInvalid()
		{
			//Arrange

			//Act
			var validator = _sut.ResetPasswordLoggedIn(string.Empty, string.Empty, string.Empty, string.Empty);

			//Assert
			Assert.True(validator.IsInvalid);
		}
		
		[Fact]
		public void ResetPasswordLoggedIn_InvalidOldPassword_ReturnsInvalid()
		{
			//Arrange
			
			_userService.Setup(x => x.GetFullAccessingUserById(USER.Id)).Returns(USER);

			//Act
			var validator = _sut.ResetPasswordLoggedIn(USER.Id, "InvalidOldPass", "NewPass", string.Empty);

			//Assert
			Assert.True(validator.IsInvalid);
		}
		
		[Fact]
		public void ResetPasswordLoggedIn_ValidOldPassword_ReturnsValid()
		{
			//Arrange
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			_userService.Setup(x => x.GetFullAccessingUserById(USER.Id)).Returns(USER);

			//Act
			var validator = _sut.ResetPasswordLoggedIn(USER.Id, USER.Password, "NewPass", string.Empty);

			//Assert
			Assert.True(validator.IsValid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void AuthenticateUserToContinue_InvalidEmail_ReturnsEmpty(string email)
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(USER.User.Email)).Returns(() => null);

			//Act
			var token = _sut.AuthenticateUserGetTokens(USER.Id, email, USER.Password);

			//Assert
			Assert.Empty(token);
		}
		
		[Fact]
		public void AuthenticateUserToContinue_DifferentUserLogin_ReturnsEmpty()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);

			//Act
			var token = _sut.AuthenticateUserGetTokens("DifferentUserId", "DifferentUserEmail", "DifferentUserPassword");

			//Assert
			Assert.Empty(token);
		}
		
		[Fact]
		public void AuthenticateUserToContinue_InvalidLogin_ReturnsEmpty()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData("InvalidPassword", It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			//Act
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, "InvalidPassword");

			//Assert
			Assert.Empty(token);
		}
		
		[Fact]
		public void AuthenticateUserToContinue_ValidLogin_ReturnsToken()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);

			//Assert
			Assert.NotEmpty(token);
		}



		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public async Task ChangeUserEmail_InvalidToken_ReturnsInvalid(string email)
		{
			//Arrange

			//Act
			var changeEmailValidation = await _sut.ChangeUserEmail(USER.Id, email, "accessToken");

			//Assert
			Assert.True(changeEmailValidation.IsInvalid);
		}

		[Fact]
		public async Task ChangeUserEmail_EmailExists_ReturnsInvalid()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);

			_userService.Setup(x => x.UserExists("newEmail")).Returns(true);

			//Act
			var validator = await _sut.ChangeUserEmail(USER.Id, "newEmail", token);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ChangeUserEmail_InvalidUser_ReturnsInvalid()
		{
			//Arrange
			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);

			_userService.Setup(x => x.GetFullAccessingUserById(It.IsAny<string>())).Returns(() => null);			

			_userService.Setup(x => x.UserExists("newEmail")).Returns(false);

			//Act
			var validator = await _sut.ChangeUserEmail(USER.Id, "newEmail", token);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ChangeUserEmail_ChangeEmailAlreadySent_ReturnsInvalid()
		{
			//Arrange
			USER.ChangeEmailTokenExpireDate = DateTime.Now.AddHours(1);

			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);


			_userService.Setup(x => x.GetFullAccessingUserById(It.IsAny<string>())).Returns(USER);

			_userService.Setup(x => x.UserExists("newEmail")).Returns(false);

			//Act
			var validator = await _sut.ChangeUserEmail(USER.Id, "newEmail", token);

			//Assert
			Assert.True(validator.IsInvalid);
		}

		[Fact]
		public async Task ChangeUserEmail_HasNoExistingChangeEmailTokenDate_ReturnsValid()
		{
			//Arrange
			USER.ChangeEmailTokenExpireDate = null;

			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);


			_userService.Setup(x => x.GetFullAccessingUserById(It.IsAny<string>())).Returns(USER);

			_userService.Setup(x => x.UserExists("newEmail")).Returns(false);

			//Act
			var validator = await _sut.ChangeUserEmail(USER.Id, "newEmail", token);

			//Assert
			Assert.True(validator.IsValid);
		}
		
		[Fact]
		public async Task ChangeUserEmail_HasExistingChangeEmailTokenDateAlreadyExpired_ReturnsValid()
		{
			//Arrange
			USER.ChangeEmailTokenExpireDate = DateTime.Now.AddHours(-1);

			_userService.Setup(x => x.GetFullAccessingUserByEmail(It.IsAny<string>())).Returns(USER);
			_encryptionService.Setup(x => x.VerifyData(USER.Password, It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var token = _sut.AuthenticateUserGetTokens(USER.Id, USER.User.Email, USER.Password);


			_userService.Setup(x => x.GetFullAccessingUserById(It.IsAny<string>())).Returns(USER);

			_userService.Setup(x => x.UserExists("newEmail")).Returns(false);

			//Act
			var validator = await _sut.ChangeUserEmail(USER.Id, "newEmail", token);

			//Assert
			Assert.True(validator.IsValid);
		}
	}
}