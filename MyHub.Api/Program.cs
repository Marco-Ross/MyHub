using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHub.Api.AppExtensions;
using MyHub.Api.AutofacModules;
using MyHub.Api.Filters;
using MyHub.Application;
using MyHub.Application.Services.Authentication;
using MyHub.Domain.Authentication;
using MyHub.Domain.ConfigurationOptions;
using MyHub.Domain.ConfigurationOptions.Authentication;
using MyHub.Domain.ConfigurationOptions.CorsOriginOptions;
using MyHub.Domain.ConfigurationOptions.Domain;
using MyHub.Domain.Exceptions;
using MyHub.Domain.RateLimiterOptions;
using MyHub.Infrastructure.Repository.EntityFramework;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.RateLimiting;

const string AllowedCorsOrigins = "_corsOrigins";
const string SlidingPolicy = "sliding";

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
var configSettings = builder.Configuration;
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AppModule(configSettings)));

//Rate Limiter
var rateOptions = new MyRateLimiterOptions();
builder.Services.AddRateLimiter(options => options.AddSlidingWindowLimiter(policyName: SlidingPolicy, options =>
{
	options.PermitLimit = rateOptions.PermitLimit;
	options.Window = rateOptions.Window;
	options.SegmentsPerWindow = rateOptions.SegmentsPerWindow;
	options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
	options.QueueLimit = rateOptions.QueueLimit;
}));

builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
{
	EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
	ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
}); //Need to use distributed sharing of these keys if ever hosting on multiple machines. Since keys are stored on machine by default (e.g. Redis, or saving in DB with EF core)

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: AllowedCorsOrigins, policy =>
	{
		policy.WithOrigins(builder.Configuration.GetSection(ConfigSections.CorsOrigin).Get<CorsOriginOptions>()?.DefaultOrigin.Split(',').Select(x => x).ToArray() ?? Array.Empty<string>())
		.SetIsOriginAllowedToAllowWildcardSubdomains()
		.SetPreflightMaxAge(TimeSpan.FromMinutes(10))
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials();
	});
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	//Replace the token header with token in cookie. (Used to Safely store the jwt on client).
	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			context.Token = context.Request.Cookies[AuthConstants.AccessTokenHeader];
			return Task.CompletedTask;
		}
	};

	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration.GetSection(ConfigSections.Authentication).Get<AuthenticationOptions>()?.JWT.Issuer ?? string.Empty,
		ValidAudience = builder.Configuration.GetSection(ConfigSections.Authentication).Get<AuthenticationOptions>()?.JWT.Audience ?? string.Empty,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection(ConfigSections.Authentication).Get<AuthenticationOptions>()?.JWT.Key ?? string.Empty)),
		ClockSkew = TimeSpan.Zero
	};
});
//So that the JWT does not map to Microsofts default claims.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();


//builder.Services.AddResponseCaching; //has to be after AddCors
builder.Services.AddAuthorization();

builder.Services.AddControllers(config =>
{
	config.Filters.Add(typeof(CsrfFilter)); //Used on all controllers
});

builder.Services.AddScoped<ApiKeyAuthFilter>(); //Added by serviceFilter attribute

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AuthenticationOptions>(builder.Configuration.GetSection(ConfigSections.Authentication));
builder.Services.Configure<DomainOptions>(builder.Configuration.GetSection(ConfigSections.Domain));
builder.Services.Configure<CorsOriginOptions>(builder.Configuration.GetSection(ConfigSections.CorsOrigin));

builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>();

builder.Services.ConfigureHttpClients(builder.Configuration);

builder.Services.AddMemoryCache();

////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	var context = services.GetRequiredService<ApplicationDbContext>();
	context.Database.Migrate();

	app.ConfigureExceptionHandler(services.GetRequiredService<ILogger<ExceptionDetails>>());
}

app.UseRateLimiter();

app.UseCors(AllowedCorsOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting(SlidingPolicy);

app.Run();
