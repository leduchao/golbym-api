
using golbym.Api.DbContext;
using Microsoft.EntityFrameworkCore;

namespace golbym.Api.UnitOfWork
{
	public class UnitOfWork<T>(AppDbContext context) : IUnitOfWork<T> where T : class
	{
		private readonly DbSet<T> _dbSet = context.Set<T>();

		public Task SaveChangesAsync()
		{
			return Task.Delay(5);
		}
	}
}
