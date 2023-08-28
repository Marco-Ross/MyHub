using FluentValidation.Results;
using MyHub.Domain.ConfigurationOptions.AdminOptions;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Domain.ConfigurationOptions.Storage;
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

		protected static StorageOptions GetStorageOptions()
		{
			return new StorageOptions
			{
				AccountName = "marcoshubstorage",
				ImageContainer = "marcohubcontainer",
				BaseFolder = "test/",
				BaseUrl = "https://marcoshubstorage.blob.core.windows.net/marcohubcontainer/",
				AccountKey = ""
			};
		}

		protected static AuthenticationOptions GetAuthOptions()
		{
			return new AuthenticationOptions
			{
				JWT = new Jwt
				{
					Key = "586E3272357538782F413F4428472B4B",
					Audience = "marcos-hub",
					Issuer = "https://localhost",
					Expiry = 0.17
				}
			};
		}
		
		protected static MarcoOptions GetMarcoOptions()
		{
			return new MarcoOptions
			{
				Email = "marcoross37@gmail.com"
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
