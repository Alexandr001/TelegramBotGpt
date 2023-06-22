using Models;
using System.Net.Http.Json;

namespace Service;

public class HttpRequestService : IHttpService
{
	private const string IP = "127.0.0.1";
	private const int PORT = 8000;

	public async Task<ResponceModel?> GetResponce(string query)
	{
		string url = new HttpEndPoint(IP, PORT, null, new Dictionary<string, string>() {
				["message"] = query
		}).ToString();
		HttpClient client = new();
		using HttpResponseMessage resp = await client.GetAsync(url);
		if (resp.IsSuccessStatusCode) {
			return await resp.Content.ReadFromJsonAsync<ResponceModel>();
		}
		return new ResponceModel() {
				Message = await resp.Content.ReadAsStringAsync()
		};
	}

	public async Task<T?> GetHttp<T>(string url)
			where T : class
	{
		HttpClient client = new();
		using HttpResponseMessage resp = await client.GetAsync(url);
		if (resp.IsSuccessStatusCode) {
			return await resp.Content.ReadFromJsonAsync<T>();
		}
		return null;
	}
}