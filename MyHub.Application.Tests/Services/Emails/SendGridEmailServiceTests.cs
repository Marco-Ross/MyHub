using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyHub.Application.Services.Emails;
using MyHub.Domain.Emails;
using MyHub.Domain.Emails.Interfaces;

namespace MyHub.Application.Tests.Services.Emails
{
	public class SendGridEmailServiceTests : ApplicationTestBase
	{
		private readonly IEmailSenderService _emailSenderService;
		private readonly Mock<ILogger<SendGridEmailService>> _logger = new();
		private readonly IOptions<AuthEmailSenderOptions> _options = Options.Create(new AuthEmailSenderOptions { SendGridKey = "Key" });
		private readonly Mock<IValidator<Email>> _emailValidator = new();

		public SendGridEmailServiceTests()
		{
			_emailSenderService = new SendGridEmailService(_logger.Object, _options, _emailValidator.Object);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public async Task SendEmailAsync_HasNoContent_ThrowsException(string content)
		{
			//Arrange

			//Act
			var sendEmailException = await Record.ExceptionAsync(async () => await _emailSenderService.SendEmailAsync(new Email(), content));

			//Assert
			Assert.IsType<NullReferenceException>(sendEmailException);
		}

		[Fact]
		public async Task SendEmailAsync_InvalidEmail_ThrowsException()
		{
			//Arrange
			_emailValidator.Setup(x => x.Validate(It.IsAny<Email>())).Returns(GetValidationError("SomeProp", "SomeError"));

			//Act
			var sendEmailException = await Record.ExceptionAsync(async () => await _emailSenderService.SendEmailAsync(null, "content"));

			//Assert
			Assert.IsType<ValidationException>(sendEmailException);
		}

		[Fact(Skip = "Key is invalid for unit test, so dont hit api.")]
		public async Task SendEmailAsync_ValidEmail_SendsEmail()
		{
			//Arrange
			_emailValidator.Setup(x => x.Validate(It.IsAny<Email>())).Returns(GetValidationResult());
			var email = new Email { To = "marcoross37@gmail.com", ToName = "Marco", Subject = "Test Subject" };

			//Act
			var sendEmailException = await Record.ExceptionAsync(async () => await _emailSenderService.SendEmailAsync(email, "content"));

			//Assert
			Assert.Null(sendEmailException);
		}
	}
}
