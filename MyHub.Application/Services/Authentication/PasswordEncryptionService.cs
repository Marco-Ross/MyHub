using Konscious.Security.Cryptography;
using MyHub.Domain.Authentication.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class PasswordEncryptionService : IPasswordEncryptionService
	{
		private readonly int KeySize = 128;
		private readonly int DegreeOfParallelism = 2; //1 cores
		private readonly int Iterations = 4;
		private readonly int MemorySize = 262000; //262MB

		public string HashPassword(string password, out byte[] salt) 
		{
			salt = RandomNumberGenerator.GetBytes(KeySize);

			return Convert.ToHexString(HashPassword(password, salt));
		}
		
		private byte[] HashPassword(string password, byte[] salt) 
		{
			var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
			{
				Salt = salt,
				DegreeOfParallelism = DegreeOfParallelism,
				Iterations = Iterations,
				MemorySize = MemorySize
			};

			return argon2.GetBytes(KeySize);
		}

		public bool VerifyPassword(string password, string hashedPassword, string salt)
		{
			var hashToCompare = HashPassword(password, Convert.FromHexString(salt));
			return hashToCompare.SequenceEqual(Convert.FromHexString(hashedPassword));
		}
	}
}
