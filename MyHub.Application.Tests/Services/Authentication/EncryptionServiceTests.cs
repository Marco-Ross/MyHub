using MyHub.Application.Services.Authentication;
using MyHub.Domain.Authentication.Interfaces;

namespace MyHub.Application.Tests.Services.Authentication
{
	public class EncryptionServiceTests : ApplicationTestBase
	{
		private readonly IEncryptionService _encryptionService;

		public EncryptionServiceTests()
		{
			_encryptionService = new EncryptionService();
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData(null)]
		public void HashData_InvalidData_ThrowsException(string data)
		{
			//Arrange

			//Act
			var hashedDataFunc = () => _encryptionService.HashData(data, out var salt);

			//Assert
			Assert.Throws<ArgumentNullException>(hashedDataFunc);
		}

		[Fact]
		public void HashData_ValidData_ReturnsHashedData()
		{
			//Arrange
			var data = "SecretData";

			//Act
			var validData = _encryptionService.HashData(data, out var salt);

			//Assert
			Assert.NotEqual(validData, data);
			Assert.NotNull(Convert.ToHexString(salt));
		}

		[Fact]
		public void VerifyData_ValidVerificationData_ReturnsTrue()
		{
			//Arrange
			var data = "SecretData";
			var hashedData = "10AF79D25632AE51EFFEF03AD327234BA765656A870428F21B8701C1870CDB5A7044C19CBF8459F62BF051BA19666EAAE4ACA1A32AF783A03CF63810AEA8A1AE51EA6C653B41CCF39B39530354E8E081FC15F130A174190B0B007AC350941470809760F4E74A9BAC3E551211F602AD109CA18541B24CB4C45A11D32BA5E84FBE";
			var salt = "F5DF70BF299E024552083C1A3269E5C3E5A839A27C9D8CCB8B9CBD04B2F96A8E9855E5863FAEBF97E5089A788B190A91279F582CCAB248FB820AFC4ABA458DC668DEC6BF9C59A4C54639F453207150B28DB1A515F5367CE912C2229B8F0859E3B1772E0F5DF56912680EFC4720FBB6862276083B875EC706422E7C87BC55FFB0";

			//Act
			var validData = _encryptionService.VerifyData(data, hashedData, salt);

			//Assert
			Assert.True(validData);
		}
	}
}
