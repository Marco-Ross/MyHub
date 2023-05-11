using Microsoft.EntityFrameworkCore;
using MyHub.Application.Services.Users;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Tests.Services.Users
{
	public class UserServiceTests : ApplicationTestBase
	{
		private readonly IUserService _userService;
		private readonly ApplicationDbContext _applicationDbContext = new(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options);
		private readonly Mock<IEncryptionService> _encryptionService = new();
		private readonly Mock<IAzureStorageService> _azureStorageService = new();
		private readonly AccessingUser USER;

		public UserServiceTests()
		{
			_baseApplicationDbContext = _applicationDbContext;
			_applicationDbContext.Database.EnsureDeleted();

			USER = new AccessingUser { Id = "TestUserId", Email = "Test@Email.com", User = new User { Username = "TestUser" }, Password = "TestPassword" };
			_userService = new UserService(_applicationDbContext, _encryptionService.Object, _azureStorageService.Object);
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
	}
}
