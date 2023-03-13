using Konscious.Security.Cryptography;
using MyHub.Domain.Authentication.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class EncryptionService : IEncryptionService
	{
		private readonly int KeySize = 128;
		private readonly int DegreeOfParallelism = 2; //1 cores
		private readonly int Iterations = 4;
		private readonly int MemorySize = 262000; //262MB

		public string HashData(string data, out byte[] salt) 
		{
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
			var hashToCompare = HashData(data, Convert.FromHexString(salt));
			return hashToCompare.SequenceEqual(Convert.FromHexString(hashedData));
		}
	}
}
