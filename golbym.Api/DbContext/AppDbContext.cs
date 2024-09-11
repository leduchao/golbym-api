using golbym.Api.Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace golbym.Api.DbContext
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
	{
		public DbSet<Post> Posts { get; init; }
		public DbSet<Tag> Tags { get; init; }
	}
}
