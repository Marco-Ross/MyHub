using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyHub.Application.Services.Authentication;
using MyHub.Application.Services.Users;
using MyHub.Domain;
using MyHub.Domain.Authentication.Interfaces;
using MyHub.Domain.Users.Interfaces;
using MyHub.Infrastructure.Repository.EntityFramework;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

const string AllowedCorsOrigins = "_corsOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: AllowedCorsOrigins, policy =>
	{
		policy.WithOrigins("https://localhost:4200")
		.AllowAnyMethod()
		.AllowAnyHeader()
		.AllowCredentials();
	});
});

builder.Services.AddHsts(options =>
{
	options.Preload = true;
	options.IncludeSubDomains = true;
	options.MaxAge = TimeSpan.FromDays(60); //usually a year
											//options.ExcludedHosts.Add("example.com");
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

builder.Services.AddHttpsRedirection(options =>
{
	options.HttpsPort = 4040;
});

//builder.Services.AddResponseCaching; //has to be after AddCors
builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(typeof(IDomainAssemblyMarker));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Move to new file and reference
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


////////////


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
else
{
	//Use HSTS in azure?
	app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseCors(AllowedCorsOrigins);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
