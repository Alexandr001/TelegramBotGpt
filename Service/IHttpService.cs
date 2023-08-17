using Models;

namespace Service;

public interface IHttpService
{
	public Task<ResponceModel?> GetResponce(string query, string docName);
	Task<T?> GetHttp<T>(string url)
			where T : class;
}