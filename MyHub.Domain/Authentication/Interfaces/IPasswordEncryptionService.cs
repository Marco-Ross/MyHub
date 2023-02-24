namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IPasswordEncryptionService
	{
		string HashPassword(string password, out byte[] salt);
		bool VerifyPassword(string password, string hashedPassword, string salt);
	}
}
