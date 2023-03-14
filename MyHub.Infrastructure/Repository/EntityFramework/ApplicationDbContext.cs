using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Emails;
using MyHub.Domain.Users;

namespace MyHub.Infrastructure.Repository.EntityFramework
{
	public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<AccessingUser> AccessingUsers { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Email> Emails { get; set; }
	}
}