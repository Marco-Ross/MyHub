using MyHub.Domain.Authentication.Interfaces;
using MyHub.Application.Services.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;

namespace MyHub.Application.Tests.Services.Authentication
{
	public class CsrfEncryptionServiceTests : ApplicationTestBase
	{
		private readonly ICsrfEncryptionService _sut;

		private readonly Mock<ILogger<CsrfEncryptionService>> _logger = new();
		private readonly IDataProtectionProvider _dataProtectionProvider = new EphemeralDataProtectionProvider();

		public CsrfEncryptionServiceTests()
		{
			_sut = new CsrfEncryptionService(_logger.Object, _dataProtectionProvider);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void Encrypt_InvalidData_ReturnsEmpty(string data)
		{
			//Arrange

			//Act
			var encryptedData = _sut.Encrypt(data);

			//Assert
			Assert.Equal(encryptedData, string.Empty);
		}
		
		[Fact]
		public void EncryptAndDecrypt_ValidData_ReturnsEncryptedData()
		{
			//Arrange
			var dataToEncrypt = "SecretData";

			//Act
			var encryptedData = _sut.Encrypt(dataToEncrypt);

			//Assert Encrypted
			Assert.NotEqual(dataToEncrypt, encryptedData);

			//Verify
			var decryptedData = _sut.Decrypt(encryptedData);

			//Assert Decrypted
			Assert.Equal(decryptedData, dataToEncrypt);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void Decrypt_InvalidData_ReturnsEmpty(string data)
		{
			//Arrange

			//Act
			var encryptedData = _sut.Decrypt(data);

			//Assert
			Assert.Equal(encryptedData, string.Empty);
		}
	}
}
