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
			var hashedData = "91C3106BFBB1AB67B548CAC0F33BBEB4C0CC142ACC06117EDF1DF6F305DD66A6C9981C8A52F12E00E7DA88C0FB67A9B77984A107C8DA0F0CFD45E5A639875152A487AC16C8992F774E5A18274F44F487B5B43E767FE0B6C91FB757F5CC6C09AE8E012451DE6B11E8355DC4A590AF15530654ACCBFF905C277B147ECC84F1F4E2";
			var salt = "F5DF70BF299E024552083C1A3269E5C3E5A839A27C9D8CCB8B9CBD04B2F96A8E9855E5863FAEBF97E5089A788B190A91279F582CCAB248FB820AFC4ABA458DC668DEC6BF9C59A4C54639F453207150B28DB1A515F5367CE912C2229B8F0859E3B1772E0F5DF56912680EFC4720FBB6862276083B875EC706422E7C87BC55FFB0";

			//Act
			var validData = _encryptionService.VerifyData(data, hashedData, salt);

			//Assert
			Assert.True(validData);
		}
	}
}
