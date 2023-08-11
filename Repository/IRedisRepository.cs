using Models;

namespace Repository;

public interface IRedisRepository
{
	public Task<T?> GetModelById<T>(long id)
			where T : class, IModel;
	public Task SetModel<T>(T model)
			where T : class, IModel;
}