namespace MyHub.Domain.Authentication.Interfaces
{
	public interface IEncryptionService
	{
		string Encrypt(string? input);
		string Decrypt(string? encryptionText);
	}
}
