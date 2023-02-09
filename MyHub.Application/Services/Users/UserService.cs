using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Authentication;
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

		public User UpdateUser(User user)
		{
			_applicationDbContext.Users.Attach(user);
			_applicationDbContext.Entry(user).State = EntityState.Modified;

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
	}
}
