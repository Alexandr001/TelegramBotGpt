using System.Text;

namespace Models.KindOfChats;

public class History
{
	public string UserMessage { get; set; } = string.Empty;
	public string BotMessage { get; set; } = string.Empty;

	public string ConvertToString()
	{
		return "Ваше сообщение:\n" + UserMessage + '\n' + 
		       "Сообщение бота:\n" + BotMessage + '\n';
	}
	
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