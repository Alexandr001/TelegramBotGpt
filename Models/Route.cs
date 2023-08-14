using System.Diagnostics.Metrics;

namespace Models;

public class Route
{
	public string ChatType { get; set; } = string.Empty;
	public string? ChatRoute { get; set; }
	public string? ChatParam { get; set; }
	public override string ToString()
	{
		if (ChatParam == null) {
			if (ChatRoute == null) {
				return '/' + ChatType;
			}
			return '/' + ChatType + '/' + ChatRoute;
		}
		return '/' + ChatType + '/' + ChatRoute + '=' + ChatParam;
	}

	public static implicit operator Route(string str)
	{
		Route route = new();
		string[] strings = str.Split(new[] {'/', '='}, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < strings.Length; i++) {
			switch (i) {
				case 0:
					route.ChatType = strings[i];
					continue;
				case 1:
					route.ChatRoute = strings[i];
					continue;
				case 2:
					route.ChatParam = strings[i];
					continue;
			}
		}
		return route;
	}
}