using System.Text;

namespace Models;

public class HttpEndPoint
{
	public string Ip { get; }
	public int? Port { get; }
	public string? Route { get; }
	public IReadOnlyDictionary<string, string>? Params { get; }

	public HttpEndPoint(string ip, 
	                    int? port = null, 
	                    string? route = null, 
	                    Dictionary<string, string>? @params = null)
	{
		Route = route;
		Port = port;
		Ip = ip;
		Params = @params;
	}

	public override string ToString()
	{
		string str = "http://" + Ip;
		if (Port != null) {
			str += ":" + Port;
		}
		if (Route != null) {
			str += Route;
		}
		if (Params != null) {
			str += AddParams(Params);
		}
		return str;
	}
	
	private static string AddParams(IReadOnlyDictionary<string, string> param)
	{
		StringBuilder sb = new("?");
		foreach ((string? key, string? value) in param) {
			sb.Append(key + "=" + value + "&");
		}
		return sb.ToString()[..^1];
	}
}