namespace golbym.Api.Repository
{
	public interface IRepository<T> where T : class
	{
		Task AddAsync(T entity);

		void Update(T entity);

		void Delete(T entity);

		Task<T?> GetByIdAsync(string id);

		IQueryable<T> GetAll();

		Task SaveChangesAsync();
	}
}
