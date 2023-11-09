using IoC;
using Models;
using Telegram.Bot;

namespace Main;

public class BotModel
{
	private static TelegramBotClient? Client { get; set; }

	public static TelegramBotClient GetTelegramBot()
	{
		if (Client is not null) { return Client; }
		
		Client = new TelegramBotClient(IoCContainer.GetConstant<string>(Constants.BOT_KEY));
		return Client;
	}
}