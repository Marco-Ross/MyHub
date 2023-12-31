﻿using FluentValidation;
using MyHub.Domain.Users.Interfaces;
using MyHub.Application.Extensions;
using MyHub.Domain.Validation.FluentValidators;

namespace MyHub.Application.Validators.Authentication
{
    public class RegisterValidator : AbstractValidator<UserRegisterValidator>
	{
		public RegisterValidator(IUsersService userservice)
		{
			RuleFor(x => userservice.UserExistsEmail(x.AccessingUser.User.Email)).Equal(false).WithMessage("This email address is already associated with Marco's hub.");
			RuleFor(x => x.AccessingUser.User.Email).NotEmpty().WithMessage("Email cannot be empty.").WithErrorCode("EmptyEmail");
			RuleFor(x => x.AccessingUser.User.Email).Must(x => x.IsValidEmail()).WithMessage("Email address is not in a valid format.").WithErrorCode("InvalidEmail");
			RuleFor(x => x.AccessingUser.User.Username).NotEmpty().WithMessage("Username cannot be empty.").WithErrorCode("EmptyUsername");
			RuleFor(x => x.AccessingUser.Password).NotEmpty().WithMessage("Password cannot be empty.").WithErrorCode("EmptyPassword");
		}
	}
}
