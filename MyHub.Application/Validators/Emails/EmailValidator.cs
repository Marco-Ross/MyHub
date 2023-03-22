using FluentValidation;
using MyHub.Domain.Emails;
using MyHub.Application.Extensions;

namespace MyHub.Application.Validators.Emails
{
	public class EmailValidator : AbstractValidator<Email>
	{
		public EmailValidator()
		{
			RuleFor(x => x).NotNull().WithMessage("Email cannot be null.").WithErrorCode("NullEmail");
			RuleFor(x => x.Subject).NotEmpty().WithMessage("Subject cannot be empty.").WithErrorCode("EmptySubject");
			RuleFor(x => x.To).NotEmpty().WithMessage("To Email Address cannot be empty.").WithErrorCode("EmptyTo");
			RuleFor(x => x.To).Must(x => x.IsValidEmail()).WithMessage("Email address is not in a valid format.").WithErrorCode("InvalidTo");
			RuleFor(x => x.ToName).NotEmpty().WithMessage("To Email Address Name cannot be empty.").WithErrorCode("EmptyToName");
		}
	}
}
