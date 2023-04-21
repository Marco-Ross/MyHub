﻿using FluentValidation.Results;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Infrastructure.Repository.EntityFramework;
using System.IdentityModel.Tokens.Jwt;

namespace MyHub.Application.Tests
{
	public class ApplicationTestBase
	{
		protected ApplicationDbContext? _baseApplicationDbContext;

		protected ApplicationTestBase()
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
		}

		protected static DomainOptions GetDomainOptions()
		{
			return new DomainOptions { Client = "testClient", Server = "testServer" };
		}
		
		protected static AuthenticationOptions GetAuthOptions()
		{
			return new AuthenticationOptions {
				JWT = new Jwt
				{
					Key = "586E3272357538782F413F4428472B4B",
					Audience = "marcos-hub",
					Issuer = "https://localhost"
				}
			};
		}

		protected static ValidationResult GetValidationResult()
		{
			return new ValidationResult();
		}

		protected static ValidationResult GetValidationError(string propertyName, string error)
		{
			var validationResult = new ValidationResult();
			validationResult.Errors.Add(new ValidationFailure(propertyName, error));
			return validationResult;
		}
		
		protected void AddToDatabase<T>(T entity)
		{
			if (entity == null) return;

			_baseApplicationDbContext?.Add(entity);
			_baseApplicationDbContext?.SaveChanges();
		}
	}
}