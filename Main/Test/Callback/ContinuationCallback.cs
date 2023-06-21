using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.Test.Callback;

public class ContinuationCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly CallbackQuery _callbackQuery;
	private readonly IAwsRepository _awsRepository;

	public ContinuationCallback(TelegramBotClient bot, CallbackQuery callbackQuery, IAwsRepository awsRepository)
	{
		_bot = bot;
		_callbackQuery = callbackQuery;
		_awsRepository = awsRepository;
	}

	public async Task ChatCallbackHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.CHAT,
				ChatRoute = "name",
				ChatParam = _callbackQuery.Data
		};
		// Получить историю чатов
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, _callbackQuery.Message!.MessageId, "Тут будет история сообщений:");
		
	}

	public async Task DocCallbackHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.DOC,
				ChatRoute = "name",
				ChatParam = _callbackQuery.Data
		};
		// Получить историю чатов
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, 
		                                _callbackQuery.Message!.MessageId, 
		                                "Тут будет история сообщений doc:");
	}
}