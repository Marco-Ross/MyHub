using FluentValidation;
using FluentValidation.TestHelper;
using MyHub.Application.Validators.Authentication;
using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Domain.Validation.FluentValidators;

namespace MyHub.Application.Tests.Validators.Authentication
{
	public class RegisterValidatorTests
	{
		private readonly RegisterValidator _registerValidator;
		private readonly Mock<IUsersService> _userService = new();

		public RegisterValidatorTests()
		{
			_registerValidator = new RegisterValidator(_userService.Object);
		}

		[Fact]
		public void RegisterValidator_UserAlreadyExists_ReturnsError()
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser());
			_userService.Setup(x => x.UserExistsEmail(It.IsAny<string>())).Returns(false);

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			Assert.False(validatorResult.IsValid);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void RegisterValidator_EmptyEmail_ReturnsError(string email)
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser { User = new User { Email = email } });

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.AccessingUser.User.Email).WithErrorCode("EmptyEmail");
		}

		[Fact]
		public void RegisterValidator_InvalidEmail_ReturnsError()
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser { User = new User { Email = "test.gmail" } });

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.AccessingUser.User.Email).WithErrorCode("InvalidEmail");
		}

		[Fact]
		public void RegisterValidator_EmptyUsername_ReturnsError()
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser());

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.AccessingUser.User.Username).WithErrorCode("EmptyUsername");
			validatorResult.ShouldHaveValidationErrorFor(x => x.AccessingUser.Password).WithErrorCode("EmptyPassword");
		}

		[Fact]
		public void RegisterValidator_EmptyPassword_ReturnsError()
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser());

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.AccessingUser.Password).WithErrorCode("EmptyPassword");
		}

		[Fact]
		public void RegisterValidator_ValidData_ReturnsValid()
		{
			//Arrange
			var userRegisterValidator = new UserRegisterValidator(new AccessingUser { User = new User { Email = "test@gmail.com", Username = "TestUser" }, Password = "TestPassword" });

			//Act
			var validatorResult = _registerValidator.TestValidate(userRegisterValidator);

			//Assert
			validatorResult.ShouldNotHaveAnyValidationErrors();
		}
	}
}
