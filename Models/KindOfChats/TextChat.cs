using System.Text;

namespace Models.KindOfChats;

public class TextChat : IChat
{
	private List<History> _history;
	public string Name { get; set; } = string.Empty;

	public List<History> ChatHistory { get; set; } = new();

	public static string HistoryListToString(List<History> list)
	{
		StringBuilder sb = new();
		foreach (History? history in list) {
			if (history != null) {
				sb.Append(history.ConvertToString());
			}
		}
		return sb.ToString();
	}
}