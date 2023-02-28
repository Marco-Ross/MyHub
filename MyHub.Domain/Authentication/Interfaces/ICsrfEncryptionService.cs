namespace MyHub.Domain.Authentication.Interfaces
{
	public interface ICsrfEncryptionService
	{
		string Encrypt(string? input);
		string Decrypt(string? encryptionText);
	}
}
