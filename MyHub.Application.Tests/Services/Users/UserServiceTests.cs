using Microsoft.EntityFrameworkCore;
using MyHub.Application.Services.Users;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Tests.Services.Users
{
	[Collection("Sequential")]
	public class UserServiceTests : ApplicationTestBase
	{
		private readonly IUsersService _userService;
		private readonly ApplicationDbContext _applicationDbContext = new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options);
		private readonly Mock<IEncryptionService> _encryptionService = new();
		private readonly Mock<IAzureStorageService> _azureStorageService = new();
		private readonly Mock<IUsersCacheService> _usersCacheService = new();
		private readonly AccessingUser USER;

		public UserServiceTests()
		{
			_baseApplicationDbContext = _applicationDbContext;
			_applicationDbContext.Database.EnsureDeleted();

			USER = new AccessingUser { Id = "TestUserId", Email = "Test@Email.com", User = new User { Id = "TestUserId", Username = "TestUser", Theme = "system-theme" }, Password = "TestPassword" };
			_userService = new UsersService(_applicationDbContext, _encryptionService.Object, _azureStorageService.Object, _usersCacheService.Object);
		}

		[Fact]
		public void RegisterUser_ValidData_SavesUser()
		{
			//Assign
			var value = Array.Empty<byte>();
			_encryptionService.Setup(x => x.HashData(It.IsAny<string>(), out value)).Returns("HashedData");

			//Act
			var registeredUser = _userService.RegisterUserDetails(USER, "TestRegisterToken");

			//Assert
			Assert.Equal(1, _applicationDbContext.AccessingUsers.Count());
			Assert.NotNull(registeredUser.Id);
		}

		[Fact]
		public void VerifyUserRegistration_InvalidRegisterToken_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser();

			//Act
			var registeredUser = _userService.VerifyUserRegistration(accessingUser, null);

			//Assert
			Assert.True(registeredUser.IsInvalid);
		}

		[Fact]
		public void VerifyUserRegistration_RegisterTokensNotMatching_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { RegisterToken = "TestRegisterToken", RegisterTokenSalt = "TestRegisterTokenSalt" };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			//Act
			var registeredUser = _userService.VerifyUserRegistration(accessingUser, "InvalidTestRegisterToken");

			//Assert
			Assert.True(registeredUser.IsInvalid);
		}

		[Fact]
		public void VerifyUserRegistration_NullRegisterTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { RegisterToken = "TestRegisterToken", RegisterTokenSalt = "TestRegisterTokenSalt", RegisterTokenExpireDate = null };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var registeredUser = _userService.VerifyUserRegistration(accessingUser, "TestRegisterToken");

			//Assert
			Assert.True(registeredUser.IsInvalid);
		}

		[Fact]
		public void VerifyUserRegistration_ExpiredRegisterTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { RegisterToken = "TestRegisterToken", RegisterTokenSalt = "TestRegisterTokenSalt", RegisterTokenExpireDate = DateTime.Now.AddHours(-1) };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var registeredUser = _userService.VerifyUserRegistration(accessingUser, "TestRegisterToken");

			//Assert
			Assert.True(registeredUser.IsInvalid);
		}

		[Fact]
		public void VerifyUserRegistration_ValidData_ReturnsValid()
		{
			//Assign
			var accessingUser = new AccessingUser { User = new User { Id = "TestUserId" }, RegisterToken = "TestRegisterToken", RegisterTokenSalt = "TestRegisterTokenSalt", RegisterTokenExpireDate = DateTime.Now.AddHours(1) };
			AddToDatabase(accessingUser);

			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var registeredUser = _userService.VerifyUserRegistration(accessingUser, "TestRegisterToken");

			//Assert
			Assert.True(registeredUser.IsValid);
			Assert.Equal(_applicationDbContext.AccessingUsers.Find("TestUserId")?.RegisterToken, string.Empty);
			Assert.Null(_applicationDbContext.AccessingUsers.Find("TestUserId")?.RegisterTokenExpireDate);
		}

		[Fact]
		public void ResetUserPassword_ValidData_ReturnsValid()
		{
			//Assign
			var accessingUser = new AccessingUser { User = new User { Id = "TestUserId" } };
			AddToDatabase(accessingUser);

			var value = Array.Empty<byte>();
			_encryptionService.Setup(x => x.HashData(It.IsAny<string>(), out value)).Returns("HashedData");

			//Act
			var resetUserPassword = _userService.ResetUserPassword(accessingUser, "TestResetPasswordToken");

			//Assert
			Assert.Equal("HashedData", _applicationDbContext.AccessingUsers.Find("TestUserId")?.ResetPasswordToken);
			Assert.NotNull(_applicationDbContext.AccessingUsers.Find("TestUserId")?.ResetPasswordTokenExpireDate);
		}

		[Fact]
		public void VerifyUserPasswordReset_InvalidResetToken_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser();

			//Act
			var verifyUserPassword = _userService.VerifyUserPasswordReset(accessingUser, null, null);

			//Assert
			Assert.True(verifyUserPassword.IsInvalid);
		}

		[Fact]
		public void VerifyUserPasswordReset_ResetTokensNotMatching_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ResetPasswordToken = "TestResetToken", ResetPasswordTokenSalt = "TestResetTokenSalt" };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			//Act
			var verifyUserPassword = _userService.VerifyUserPasswordReset(accessingUser, "TestPassword", "InvalidTestResetToken");

			//Assert
			Assert.True(verifyUserPassword.IsInvalid);
		}

		[Fact]
		public void VerifyUserPasswordReset_NullResetPasswordTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ResetPasswordToken = "TestResetToken", ResetPasswordTokenSalt = "TestResetTokenSalt", ResetPasswordTokenExpireDate = null };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var verifyUserPassword = _userService.VerifyUserPasswordReset(accessingUser, "TestPassword", "TestResetToken");

			//Assert
			Assert.True(verifyUserPassword.IsInvalid);
		}

		[Fact]
		public void VerifyUserPasswordReset_ExpiredResetPasswordTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ResetPasswordToken = "TestResetToken", ResetPasswordTokenSalt = "TestResetTokenSalt", ResetPasswordTokenExpireDate = DateTime.Now.AddHours(-1) };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var verifyUserPassword = _userService.VerifyUserPasswordReset(accessingUser, "TestPassword", "TestResetToken");

			//Assert
			Assert.True(verifyUserPassword.IsInvalid);
		}

		[Fact]
		public void VerifyUserPasswordReset_ValidData_ReturnsValid()
		{
			//Assign
			var accessingUser = new AccessingUser { User = new User { Id = "TestUserId" }, ResetPasswordToken = "TestResetToken", ResetPasswordTokenSalt = "TestResetTokenSalt", ResetPasswordTokenExpireDate = DateTime.Now.AddHours(1) };
			AddToDatabase(accessingUser);

			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			var value = Array.Empty<byte>();
			_encryptionService.Setup(x => x.HashData(It.IsAny<string>(), out value)).Returns("TestPassword");

			//Act
			var verifyUserPassword = _userService.VerifyUserPasswordReset(accessingUser, "TestPassword", "TestResetToken");

			//Assert
			Assert.True(verifyUserPassword.IsValid);
			Assert.Equal(_applicationDbContext.AccessingUsers.Find("TestUserId")?.ResetPasswordToken, string.Empty);
			Assert.Null(_applicationDbContext.AccessingUsers.Find("TestUserId")?.ResetPasswordTokenExpireDate);
		}

		[Fact]
		public void RevokeUser_NullUser_ReturnsInvalid()
		{
			//Assign

			//Act
			var revokeUser = _userService.RevokeUser(string.Empty, string.Empty);

			//Assert
			Assert.Null(revokeUser);
		}

		[Fact]
		public void RevokeUser_NullRefreshTokenId_ReturnsInvalid()
		{
			//Assign
			var accessingUser = new AccessingUser { User = new User { Id = "TestUserId" } };
			AddToDatabase(accessingUser);

			//Act
			var revokeUser = _userService.RevokeUser(accessingUser.User.Id, string.Empty);

			//Assert
			Assert.Null(revokeUser);
		}

		[Fact]
		public void RevokeUser_HasUser_ReturnsValid()
		{
			//Assign
			var accessingUser = new AccessingUser
			{
				User = new User { Id = "TestUserId" },
				RefreshTokens = new List<RefreshToken> { new RefreshToken { Token = "TestRefreshToken" } }
			};

			AddToDatabase(accessingUser);

			//Act
			_userService.RevokeUser(accessingUser.Id, accessingUser.RefreshTokens.First().Token);

			//Assert
			Assert.False(_applicationDbContext.AccessingUsers.Find(accessingUser.Id)?.RefreshTokens.Any());
		}

		[Fact]
		public void UpdateUserTheme_UserDoesNotExist_NotSaved()
		{
			//Assign

			//Act
			_userService.UpdateUserTheme(string.Empty, USER.User.Theme);

			//Assert
			Assert.Null(_applicationDbContext.Users.Find(USER.Id)?.Theme);
		}

		[Fact]
		public void UpdateUserTheme_UserExists_Saved()
		{
			//Assign
			AddToDatabase(USER.User);

			//Act
			_userService.UpdateUserTheme(USER.Id, USER.User.Theme);

			//Assert
			Assert.NotNull(_applicationDbContext.Users.Find(USER.Id)?.Theme);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void GetUserTheme_NoUserId_ReturnsEmpty(string theme)
		{
			//Assign

			//Act
			_userService.GetUserTheme(theme);

			//Assert
			Assert.Null(_applicationDbContext.Users.Find(USER.Id)?.Theme);
		}

		[Fact]
		public void GetUserTheme_UserId_ReturnsTheme()
		{
			//Assign
			AddToDatabase(USER.User);

			//Act
			var loadedTheme = _userService.GetUserTheme(USER.Id);

			//Assert
			Assert.Equal(USER.User.Theme, loadedTheme);
		}

		[Fact]
		public void UpdateUserAccount_UserId_ReturnsTheme()
		{
			//Assign
			AddToDatabase(USER);

			var newUserDetails = new AccessingUser { User = new User { Username = "newUsername", Name = "newName", Surname = "newSurname" } };

			//Act
			_userService.UpdateUserAccount(newUserDetails, USER.Id);

			//Assert
			Assert.Equal(newUserDetails.User.Username, _applicationDbContext.AccessingUsers.Find(USER.Id)?.User.Username);
			Assert.Equal(newUserDetails.User.Name, _applicationDbContext.AccessingUsers.Find(USER.Id)?.User.Name);
			Assert.Equal(newUserDetails.User.Surname, _applicationDbContext.AccessingUsers.Find(USER.Id)?.User.Surname);
		}

		[Fact]
		public void ChangeUserEmailComplete_InvalidChangeEmailToken_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser();

			//Act
			var verifyUserEmailChange = _userService.ChangeUserEmailComplete(accessingUser, string.Empty);

			//Assert
			Assert.True(verifyUserEmailChange.IsInvalid);
		}

		[Fact]
		public void ChangeUserEmailComplete_ChangeEmailTokensNotMatching_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ChangeEmailToken = "TestEmailChangeToken", ChangeEmailTokenSalt = "TestEmailChangeTokenSalt" };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

			//Act
			var verifyUserEmailChange = _userService.ChangeUserEmailComplete(accessingUser, "InvalidEmailChangeToken");

			//Assert
			Assert.True(verifyUserEmailChange.IsInvalid);
		}

		[Fact]
		public void ChangeUserEmailComplete_NullEmailChangeTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ChangeEmailToken = "TestEmailChangeToken", ChangeEmailTokenSalt = "TestEmailChangeTokenSalt", ChangeEmailTokenExpireDate = null };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var verifyUserEmailChange = _userService.ChangeUserEmailComplete(accessingUser, "TestEmailChangeToken");

			//Assert
			Assert.True(verifyUserEmailChange.IsInvalid);
		}

		[Fact]
		public void ChangeUserEmailComplete_ExpiredEmailChangeTokenExpireDate_ReturnsError()
		{
			//Assign
			var accessingUser = new AccessingUser { ChangeEmailToken = "TestEmailChangeToken", ChangeEmailTokenSalt = "TestEmailChangeTokenSalt", ChangeEmailTokenExpireDate = DateTime.Now.AddHours(-1) };
			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var verifyUserEmailChange = _userService.ChangeUserEmailComplete(accessingUser, "TestEmailChangeToken");

			//Assert
			Assert.True(verifyUserEmailChange.IsInvalid);
		}

		[Fact]
		public void ChangeUserEmailComplete_ValidData_ReturnsValid()
		{
			//Assign
			var accessingUser = new AccessingUser { User = new User { Id = "TestUserId" }, ChangeEmailToken = "TestEmailChangeToken", ChangeEmailTokenSalt = "TestEmailChangeTokenSalt", ChangeEmailTokenExpireDate = DateTime.Now.AddHours(1) };
			AddToDatabase(accessingUser);

			_encryptionService.Setup(x => x.VerifyData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

			//Act
			var verifyUserEmailChange = _userService.ChangeUserEmailComplete(accessingUser, "TestEmailChangeToken");

			//Assert
			Assert.True(verifyUserEmailChange.IsValid);
			Assert.Equal(_applicationDbContext.AccessingUsers.Find("TestUserId")?.ChangeEmailToken, string.Empty);
			Assert.Null(_applicationDbContext.AccessingUsers.Find("TestUserId")?.ChangeEmailTokenExpireDate);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void DeleteUser_InvalidUserId_NotRemoved(string userId)
		{
			//Assign
			AddToDatabase(USER.User);

			//Act
			_userService.DeleteUser(userId);

			//Assert
			Assert.NotNull(_applicationDbContext.Users.Find(USER.User.Id));
		}

		[Fact]
		public void DeleteUser_ValidUserId_Removed()
		{
			//Assign
			AddToDatabase(USER.User);

			//Act
			_userService.DeleteUser(USER.User.Id);

			//Assert
			Assert.Null(_applicationDbContext.Users.Find(USER.User.Id));
		}
	}
}
