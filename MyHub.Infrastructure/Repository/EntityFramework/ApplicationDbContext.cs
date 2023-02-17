using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Users;

namespace MyHub.Infrastructure.Repository.EntityFramework
{
    public class ApplicationDbContext : DbContext
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
	}
}
