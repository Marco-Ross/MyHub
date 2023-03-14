namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IEncryptionService
	{
		string GenerateSecureToken();
		string HashData(string data, out byte[] salt);
		bool VerifyData(string data, string hashedData, string salt);
	}
}
