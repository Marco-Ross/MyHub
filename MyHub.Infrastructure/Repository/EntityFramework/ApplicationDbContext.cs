﻿using Microsoft.EntityFrameworkCore;
using MyHub.Domain.Authentication;
using MyHub.Domain.Emails;
using MyHub.Domain.Feedback;
using MyHub.Domain.Gallery;
using MyHub.Domain.Titbits;
using MyHub.Domain.Users;

namespace MyHub.Infrastructure.Repository.EntityFramework
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Email>().UseTptMappingStrategy();
		}

		public DbSet<AccessingUser> AccessingUsers { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<ThirdPartyDetails> ThirdPartyDetails { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Email> Emails { get; set; }
		public DbSet<AccountRegisterEmail> AccountRegisterEmails { get; set; }
		public DbSet<PasswordRecoveryEmail> PasswordRecoveryEmails { get; set; }
		public DbSet<GalleryImage> GalleryImages { get; set; }
		public DbSet<GalleryImageComment> GalleryImageComments { get; set; }
		public DbSet<Titbit> Titbits { get; set; }
		public DbSet<TitbitLink> TitbitLinks { get; set; }
		public DbSet<TitbitCategory> TitbitCategories { get; set; }
		public DbSet<UserFeedback> Feedback { get; set; }
	}
}