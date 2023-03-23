using MyHub.Application.Validators.Emails;
using FluentValidation.TestHelper;
using MyHub.Domain.Emails;

namespace MyHub.Application.Tests.Validators.Emails
{
	public class EmailValidatorTests
	{
		private readonly EmailValidator _emailValidator;

		public EmailValidatorTests()
		{
			_emailValidator = new EmailValidator();
		}

		[Fact]
		public void EmailValidator_EmptySubject_ReturnsError()
		{
			//Arrange
			var email = new Email();

			//Act
			var validatorResult = _emailValidator.TestValidate(email);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.Subject).WithErrorCode("EmptySubject");
		}

		[Fact]
		public void EmailValidator_EmptyTo_ReturnsError()
		{
			//Arrange
			var email = new Email();

			//Act
			var validatorResult = _emailValidator.TestValidate(email);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.To).WithErrorCode("EmptyTo");
		}

		[Fact]
		public void EmailValidator_InvalidTo_ReturnsError()
		{
			//Arrange
			var email = new Email { To = "test.gmail" };

			//Act
			var validatorResult = _emailValidator.TestValidate(email);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.To).WithErrorCode("InvalidTo");
		}

		[Fact]
		public void EmailValidator_EmptyToName_ReturnsError()
		{
			//Arrange
			var email = new Email();

			//Act
			var validatorResult = _emailValidator.TestValidate(email);

			//Assert
			validatorResult.ShouldHaveValidationErrorFor(x => x.ToName).WithErrorCode("EmptyToName");
		}

		[Fact]
		public void EmailValidator_ValidData_ReturnsValid()
		{
			//Arrange
			var email = new Email { Subject = "TestSubject", To = "test@gmail.com", ToName = "TestUser" };

			//Act
			var validatorResult = _emailValidator.TestValidate(email);

			//Assert
			validatorResult.ShouldNotHaveAnyValidationErrors();
		}
	}
}
