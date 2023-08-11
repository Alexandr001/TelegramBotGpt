using IoC;
using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.View.Callback;

public class ContinuationCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly IAwsRepository _awsRepository;

	public ContinuationCallback()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_awsRepository = IoCContainer.GetService<IAwsRepository>();
	}

	public async Task ChatCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.CHAT,
				ChatRoute = MainRouteConstants.NAME,
				ChatParam = callbackQuery.Data
		};
		// ToDo: Получить историю чатов
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!, callbackQuery.Message!.MessageId, "Тут будет история сообщений:");
	}

	public async Task DocCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.DOC,
				ChatRoute = MainRouteConstants.NAME,
				ChatParam = callbackQuery.Data
		};
		// ToDo: Получить историю чатов
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!, callbackQuery.Message!.MessageId, "Тут будет история сообщений doc:");
	}
}