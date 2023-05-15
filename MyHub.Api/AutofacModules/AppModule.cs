using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyHub.Api.Filters;
using MyHub.Application;
using MyHub.Application.Hubs;
using MyHub.Application.Services.Authentication;
using MyHub.Application.Services.BackgroundServices;
using MyHub.Application.Services.Emails;
using MyHub.Application.Services.Emails.EmailConstructors;
using MyHub.Application.Services.Integration.AzureDevOps;
using MyHub.Application.Services.Integration.AzureStorage;
using MyHub.Application.Services.Users;
using MyHub.Domain;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Background.CleanBackground.Interfaces;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Domain.Hubs.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.Interfaces;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;

namespace MyHub.Api.AutofacModules
{
	public class AppModule : Module
	{
		private readonly IConfiguration _configuration;
		public AppModule(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(x =>
			{
				var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
				optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
				return new ApplicationDbContext(optionsBuilder.Options);

			}).InstancePerLifetimeScope();

			builder.RegisterAutoMapper(typeof(IDomainAssemblyMarker).Assembly);

			builder.Register<IEmailConstructorFactory>(c => new EmailConstructorFactory(c.Resolve<IComponentContext>()));
			builder.RegisterType<AccountRegisterEmailConstructor>();
			builder.RegisterType<PasswordEmailConstructor>();

			builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
			builder.RegisterType<UserService>().As<IUsersService>().InstancePerLifetimeScope();
			builder.RegisterType<CsrfEncryptionService>().As<ICsrfEncryptionService>().InstancePerLifetimeScope();
			builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerLifetimeScope();
			builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
			builder.RegisterType<SendGridEmailService>().As<IEmailSenderService>().InstancePerLifetimeScope();
			builder.RegisterType<ApiKeyAuthFilter>().InstancePerLifetimeScope();
			builder.RegisterType<AzureDevOpsCacheService>().As<IAzureDevOpsCacheService>().InstancePerLifetimeScope();
			builder.RegisterType<CleanTokensBackgroundService>().As<ICleanTokensBackgroundService>().InstancePerLifetimeScope();
			builder.RegisterAssemblyTypes(typeof(IApplicationAssemblyMarker).Assembly).AsClosedTypesOf(typeof(IHubResolver<>)).InstancePerLifetimeScope();
			builder.RegisterType<UserIdProvider>().As<IUserIdProvider>().SingleInstance();
			builder.RegisterType<AzureStorageService>().As<IAzureStorageService>().SingleInstance();

			//CacheDecorators
			builder.RegisterDecorator<AzureDevOpsCacheService, IAzureDevOpsService>();
			builder.RegisterDecorator<UsersCacheService, IUsersService>();
		}
	}
}
