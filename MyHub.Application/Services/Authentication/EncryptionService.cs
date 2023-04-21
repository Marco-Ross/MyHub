using Konscious.Security.Cryptography;
using MyHub.Domain.Authentication.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class EncryptionService : IEncryptionService
	{
		private readonly int KeySize = 128;
		private readonly int DegreeOfParallelism = 2;
		private readonly int Iterations = 2;
		private readonly int MemorySize = 10000; //10MB

		public string HashData(string data, out byte[] salt) 
		{
			if(string.IsNullOrWhiteSpace(data))
				throw new ArgumentNullException(nameof(data));

			salt = RandomNumberGenerator.GetBytes(KeySize);

			return Convert.ToHexString(HashData(data, salt));
		}
		
		private byte[] HashData(string data, byte[] salt) 
		{
			var argon2 = new Argon2id(Encoding.UTF8.GetBytes(data))
			{
				Salt = salt,
				DegreeOfParallelism = DegreeOfParallelism,
				Iterations = Iterations,
				MemorySize = MemorySize
			};

			return argon2.GetBytes(KeySize);
		}

		public bool VerifyData(string data, string hashedData, string salt)
		{
			if (string.IsNullOrWhiteSpace(data))
				throw new ArgumentNullException(nameof(data));
			
			if (string.IsNullOrWhiteSpace(data))
				throw new ArgumentNullException(nameof(data));

			var hashToCompare = HashData(data, Convert.FromHexString(salt));
			return hashToCompare.SequenceEqual(Convert.FromHexString(hashedData));
		}

		public string GenerateSecureToken()
		{
			var key = new byte[32];
			RandomNumberGenerator.Create().GetBytes(key);
			var base64Secret = Convert.ToBase64String(key);

			// make safe for url
			return base64Secret.TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}
	}
}
