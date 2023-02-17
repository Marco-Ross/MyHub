using MyHub.Domain.Users;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Users
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _applicationDbContext;

		public UserService(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		public User CreateUser(User user)
		{
			user.Id = Guid.NewGuid().ToString();

			//save encrypted password

			var savedUser = _applicationDbContext.Add(user);
			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public User? RevokeUser(string userId)
		{
			var user = GetUser(userId);

			if (user == null)
				return null;

			user.RefreshToken = string.Empty;

			_applicationDbContext.SaveChanges();

			return user;
		}

		public User? RevokeUser(User user)
		{
			if (user == null)
				return null;

			user.RefreshToken = string.Empty;

			_applicationDbContext.SaveChanges();

			return user;
		}

		public User? GetUser(string id)
		{
			return _applicationDbContext.Find<User>(id);
		}

		public User? GetUserWithCredentials(string username, string password)
		{
			//password = password ?? string.Empty; // hashed password.
			//user doesnt exist exception

			return _applicationDbContext.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
		}

		public void UpdateRefreshToken(User authenticatingUser, string refreshToken)
		{
			authenticatingUser.RefreshToken = refreshToken;

			_applicationDbContext.SaveChanges();
		}
	}
}
