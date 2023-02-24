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
			user.Id = Guid.NewGuid().ToString();

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
		
		public bool UserExists(string email)
		{
			return _applicationDbContext.Users.Any(x => x.Email == email);
		}

		public User? GetUserByEmail(string email)
		{
			return _applicationDbContext.Users.SingleOrDefault(x => x.Email == email);
		}

		public void UpdateRefreshToken(User authenticatingUser, string refreshToken)
		{
			authenticatingUser.RefreshToken = refreshToken;

			_applicationDbContext.SaveChanges();
		}
	}
}
