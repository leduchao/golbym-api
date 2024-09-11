using golbym.Api.DbContext;
using golbym.Api.Endpoints;
using golbym.Api.Repository;
using golbym.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using golbym.Api.Domains;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.Configure<JsonOptions>(options =>
{
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	//options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDatabase"));
});

builder.Services
	.AddIdentity<AppUser, IdentityRole>(options =>
	{
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 6;
		options.Password.RequireUppercase = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireDigit = false;
	})
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
			ValidAudience = builder.Configuration.GetSection("Jwt:Issuer").Value,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!))
		};
	});

builder.Services
	.AddAuthorizationBuilder()
	.AddPolicy("admin_role", policy =>
	{
		policy.RequireRole("Admin");
	});

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(config =>
	{
		config
			.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PostRepository>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
	RequestPath = "/uploads"
});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapPostEndpoints();
app.MapAuthEndpoint();

await app.RunAsync();
