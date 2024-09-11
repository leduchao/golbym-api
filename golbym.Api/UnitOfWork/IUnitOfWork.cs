namespace golbym.Api.UnitOfWork
{
	public interface IUnitOfWork<T> where T : class
	{
		Task SaveChangesAsync();
	}
}
