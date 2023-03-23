using FluentValidation.Results;
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

		protected static Dictionary<string, string?> GetConfigValues()
		{
			return new Dictionary<string, string?> {
				{ "JWT:Key", "586E3272357538782F413F4428472B4B"},
				{ "JWT:Audience", "marcos-hub"},
				{ "JWT:Issuer", "https://localhost"}
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
