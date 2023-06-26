using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.Test.Callback;

public class CreationCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly CallbackQuery _callbackQuery;
	private readonly IAwsRepository _awsRepository;

	public CreationCallback(TelegramBotClient bot, CallbackQuery callbackQuery, IAwsRepository awsRepository)
	{
		_bot = bot;
		_callbackQuery = callbackQuery;
		_awsRepository = awsRepository;
	}
	public async Task ChatCallbackHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.CHAT,
				ChatRoute = "new",
		};
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, 
		                                _callbackQuery.Message!.MessageId, 
		                                "Введите название чата:");
	}

	public async Task DocCallbackHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.DOC,
				ChatRoute = "new",
		};
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, 
		                                _callbackQuery.Message!.MessageId, 
		                                "Введите название чата и прикрепите документ:");
	}
}