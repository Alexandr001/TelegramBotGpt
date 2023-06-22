namespace Service;

public interface IHttpService
{
	Task<T?> GetHttp<T>(string url)
			where T : class;
}