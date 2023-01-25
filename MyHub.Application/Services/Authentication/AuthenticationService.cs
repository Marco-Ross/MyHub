using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Application.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		public AuthenticationService(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		public User CreateUser(User user)
		{
			user.Id = Guid.NewGuid();

			var savedUser = _applicationDbContext.Add(user);
			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}
	}
}
