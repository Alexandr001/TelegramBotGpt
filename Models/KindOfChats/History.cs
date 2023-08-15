namespace Models.KindOfChats;

public class History
{
	public string UserMessages { get; set; } = string.Empty;
	public string BotMessages { get; set; } = string.Empty;

	public string ConvertToString()
	{
		return "Ваше сообщение:\n" + UserMessages + '\n' + 
		       "Сообщение бота:\n" + BotMessages + '\n';
	}
}