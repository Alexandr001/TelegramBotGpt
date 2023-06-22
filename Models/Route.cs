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

	public static Route Parse(string? chatType = null, string? str = null)
	{
		Route route = new() {
				ChatType = chatType?.Trim('/') ?? string.Empty
		};
		if (str == null) {
			return route;
		}
		int indexOf = str.IndexOf('=');
		if (indexOf == -1) {
			route.ChatRoute = str;
			return route;
		}
		route.ChatRoute = str[..(indexOf)];
		route.ChatParam = str[(indexOf + 1)..];
		return route;
	}
}