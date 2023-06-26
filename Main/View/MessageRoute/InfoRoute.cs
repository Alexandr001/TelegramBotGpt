using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.View.MessageRoute;

public class InfoRoute : IRoute
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public InfoRoute(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}
	public async Task RouteHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.INFO
		};
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Тут будет текст описания бота!");
	}
}