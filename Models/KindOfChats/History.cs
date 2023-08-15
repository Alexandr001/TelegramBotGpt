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
}