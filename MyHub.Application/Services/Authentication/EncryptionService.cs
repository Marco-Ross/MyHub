using Microsoft.AspNetCore.DataProtection;
using MyHub.Domain.Authentication.Interfaces;

namespace MyHub.Application.Services.Authentication
{
	public class EncryptionService : IEncryptionService
	{
		private readonly IDataProtectionProvider _dataProtectionProvider;
		private const string Protector = "MyHubProtector";

		public EncryptionService(IDataProtectionProvider dataProtectionProvider)
		{
			_dataProtectionProvider = dataProtectionProvider;
		}
		public string Encrypt(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return string.Empty;

			var protector = _dataProtectionProvider.CreateProtector(Protector);
			return protector.Protect(input);
		}

		public string Decrypt(string? encryptionText)
		{
			if (string.IsNullOrWhiteSpace(encryptionText))
				return string.Empty;

			var protector = _dataProtectionProvider.CreateProtector(Protector);
			return protector.Unprotect(encryptionText);
		}	
	}
}
