﻿using Autofac;
using Autofac.Core;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyHub.Api.Authorization;
using MyHub.Api.AutofacModules.Factories;
using MyHub.Api.Filters;
using MyHub.Application;
using MyHub.Application.Hubs;
using MyHub.Application.Services.Attachment;
using MyHub.Application.Services.Authentication;
using MyHub.Application.Services.BackgroundServices;
using MyHub.Application.Services.Emails;
using MyHub.Application.Services.Emails.EmailConstructors;
using MyHub.Application.Services.Feedback;
using MyHub.Application.Services.Gallery;
using MyHub.Application.Services.Images;
using MyHub.Application.Services.Integration.AzureDevOps;
using MyHub.Application.Services.Integration.AzureStorage;
using MyHub.Application.Services.Sanitize;
using MyHub.Application.Services.Titbits;
using MyHub.Application.Services.Users;
using MyHub.Domain;
using MyHub.Domain.Attachment.Interfaces;
using MyHub.Domain.Authentication;
using MyHub.Domain.Authentication.Github;
using MyHub.Domain.Authentication.Google;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Background.CleanBackground.Interfaces;
using MyHub.Domain.Emails.Interfaces;
using MyHub.Domain.Feedback.Interfaces;
using MyHub.Domain.Gallery.Interfaces;
using MyHub.Domain.Hubs.Interfaces;
using MyHub.Domain.Images.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureStorage.Interfaces;
using MyHub.Domain.Integration.AzureDevOps.AzureWorkItems.Interfaces;
using MyHub.Domain.Sanitizer.Interfaces;
using MyHub.Domain.Titbits.Interfaces;
using MyHub.Domain.Users.Google;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;
using Octokit;

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
				optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"), builder =>
				{
					builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(20), null);
				});
				return new ApplicationDbContext(optionsBuilder.Options);

			}).InstancePerLifetimeScope();

			builder.RegisterAutoMapper(typeof(IDomainAssemblyMarker).Assembly);

			builder.Register<IEmailConstructorFactory>(c => new EmailConstructorFactory(c.Resolve<IComponentContext>()));
			builder.RegisterType<AccountRegisterEmailConstructor>();
			builder.RegisterType<PasswordEmailConstructor>();
			builder.RegisterType<ChangeEmailConstructor>();

			builder.RegisterType<UsersService>().As<IUsersService>().InstancePerLifetimeScope();
			builder.RegisterType<UsersCacheService>().As<IUsersCacheService>().InstancePerLifetimeScope();
			builder.RegisterType<GoogleUsersService>().As<IGoogleUsersService>().InstancePerLifetimeScope();
			builder.RegisterType<AuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
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
			builder.RegisterType<ImageService>().As<IImageService>().InstancePerLifetimeScope();
			builder.RegisterType<GoogleAuthenticationService>().As<IGoogleAuthenticationService>().InstancePerLifetimeScope();
			builder.RegisterType<GithubAuthenticationService>().As<IGithubAuthenticationService>().InstancePerLifetimeScope();
			builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
			builder.RegisterType<GalleryService>().As<IGalleryService>().InstancePerLifetimeScope();
			builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerLifetimeScope();
			builder.RegisterType<UserGalleryService>().As<IUserGalleryService>().InstancePerLifetimeScope();
			builder.RegisterType<MarcoService>().As<IMarcoService>().InstancePerLifetimeScope();
			builder.RegisterType<TitbitsService>().As<ITitbitsService>().InstancePerLifetimeScope();
			builder.RegisterType<FeedbackService>().As<IFeedbackService>().InstancePerLifetimeScope();
			builder.RegisterType<SanitizerService>().As<ISanitizerService>().InstancePerLifetimeScope();

			builder.Register<IGitHubClient>(c =>
			{
				var httpContextAccessor = c.Resolve<IHttpContextAccessor>();
				var accessToken = httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.AccessToken];

				var githubClient = new GitHubClient(new ProductHeaderValue("MarcosHub"));

				if (!string.IsNullOrWhiteSpace(accessToken))
					githubClient.Credentials = new Credentials(accessToken ?? string.Empty);

				return githubClient;

			}).InstancePerLifetimeScope();

			//CacheDecorators
			builder.RegisterDecorator<AzureDevOpsCacheService, IAzureDevOpsService>();
		}
	}
}
