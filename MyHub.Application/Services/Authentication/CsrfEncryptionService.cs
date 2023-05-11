using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using MyHub.Domain.Authentication.Interfaces;
using System.Security.Cryptography;

namespace MyHub.Application.Services.Authentication
{
	public class CsrfEncryptionService : ICsrfEncryptionService
	{
		private readonly ILogger _logger;
		private readonly IDataProtectionProvider _dataProtectionProvider;
		private const string Protector = "MyHubProtector";

		public CsrfEncryptionService(ILogger<CsrfEncryptionService> logger, IDataProtectionProvider dataProtectionProvider)
		{
			_logger = logger;
			_dataProtectionProvider = dataProtectionProvider;
		}
		public string Encrypt(string? input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return string.Empty;

			try
			{
				var protector = _dataProtectionProvider.CreateProtector(Protector);
				return protector.Protect(input);
			}
			catch (CryptographicException e)
			{
				_logger.LogError(e, "CSRF encryption failed.");
				return string.Empty;
			}
		}

		public string Decrypt(string? encryptionText)
		{
			if (string.IsNullOrWhiteSpace(encryptionText))
				return string.Empty;

			try
			{
				var protector = _dataProtectionProvider.CreateProtector(Protector);
				return protector.Unprotect(encryptionText);
			}
			catch (CryptographicException e)
			{
				_logger.LogError(e, "CSRF decryption failed.");
				return string.Empty;
			}
		}
	}
}
