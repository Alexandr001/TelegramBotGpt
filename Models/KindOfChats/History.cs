using System.Text;

namespace Models.KindOfChats;

public class History
{
	public List<string> UserMessages { get; set; } = new();
	public List<string> BotMessages { get; set; } = new();

	public override string ToString()
	{
		StringBuilder sb = new();
		if (UserMessages.Count != BotMessages.Count) {
			return "История чатов сломалась :(";
		}
		for (int i = UserMessages.Count - 1; i >= 0; i--) {
			sb.Append("Ваше сообщение:\n" + UserMessages[i]);
			sb.Append("Сообщение бота:\n" + BotMessages[i]);	
		}
		return sb.ToString();
	}
}