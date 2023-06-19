using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Models;

namespace Repository;

public class RedisRepository
{
	private readonly IDistributedCache _distributedCache;

	public RedisRepository(IDistributedCache distributedCache)
	{
		_distributedCache = distributedCache;
	}

	public async Task<T?> GetModelById<T>(long id) where T : class, IModel
	{
		string? stringAsync = await _distributedCache.GetStringAsync(id.ToString());
		return stringAsync != null 
				       ? JsonSerializer.Deserialize<T>(stringAsync) 
				       : null;
	}

	public async Task SetModel<T>(T model) where T : class, IModel
	{
		string serialize = JsonSerializer.Serialize(model);
		await _distributedCache.SetStringAsync(model.Id.ToString(), serialize);
	}
}