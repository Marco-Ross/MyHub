using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Authentication;

namespace MyHub.Infrastructure.Repository.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<User> User { get; set; }
	}
}
