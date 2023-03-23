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
using MyHub.Application;
using MyHub.Application.Services.Authentication;
using MyHub.Domain.Emails;
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
		policy.WithOrigins("https://*.marcoshub.com", "https://localhost:4200")//maybe no local host in prod?
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
			context.Token = context.Request.Cookies["X-Access-Token"];
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
		ValidIssuer = builder.Configuration["JWT:Issuer"],
		ValidAudience = builder.Configuration["JWT:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ?? string.Empty)),
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
	config.Filters.Add(typeof(CsrfFilter));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AuthEmailSenderOptions>(builder.Configuration.GetSection("AuthEmailSenderOptions"));
builder.Services.AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>();

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
