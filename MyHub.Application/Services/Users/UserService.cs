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

		public User RegisterUser(User user)
		{
			var savedUser = _applicationDbContext.Add(user);
			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public User? RevokeUser(string userId) => RevokeUser(GetFullUserById(userId));

		public User? RevokeUser(User? user)
		{
			if (user == null)
				return null;

			user.RefreshToken = string.Empty;

			_applicationDbContext.SaveChanges();

			return user;
		}

		public bool UserExists(string email) => _applicationDbContext.Users.Any(x => x.Email == email);

		public User? GetFullUserByEmail(string email) => _applicationDbContext.Users.SingleOrDefault(x => x.Email == email);

		public User? GetFullUserById(string id) => _applicationDbContext.Find<User>(id);

		public void UpdateRefreshToken(User authenticatingUser, string refreshToken)
		{
			authenticatingUser.RefreshToken = refreshToken;

			_applicationDbContext.SaveChanges();
		}
	}
}
