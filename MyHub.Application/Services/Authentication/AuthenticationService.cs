using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyHub.Application.Services.Authentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IConfiguration _configuration;

		public AuthenticationService(ApplicationDbContext applicationDbContext, IConfiguration configuration)
		{
			_applicationDbContext = applicationDbContext;
			_configuration = configuration;
		}

		public User CreateUser(User user)
		{
			user.Id = Guid.NewGuid().ToString();

			//save encrypted password

			var savedUser = _applicationDbContext.Add(user);
			_applicationDbContext.SaveChanges();

			return savedUser.Entity;
		}

		public Tokens AuthenticateUser(User user)
		{
			var loadedUser = _applicationDbContext.Find<User>(user.Id);

			if (loadedUser is null)
				return null;

			//check against encrypted password


			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? string.Empty);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _configuration["JWT:Audience"],
				Issuer = _configuration["JWT:Issuer"],
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim("Id", user.Id.ToString()),
					new Claim(JwtRegisteredClaimNames.Sub, user.Username),
					new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
					//new Claim(JwtRegisteredClaimNames.Email, user.Email)
				}),
				Expires = DateTime.UtcNow.AddMinutes(15), //Single sing on?
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new Tokens { Token = tokenHandler.WriteToken(token) };
		}
	}
}
