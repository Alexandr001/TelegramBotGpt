using Models;
using Models.KindOfChats;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Route = Models.Route;

namespace Main.Test.MessageRoute;

public class DocRoute : IRoute
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public DocRoute(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}
	public async Task RouteHandler(ChatModelForUser model)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.DOC
		};
		model.ChatType = MainRouteConstants.DOC;
		List<InlineKeyboardButton[]> list = new() {
				new[] {InlineKeyboardButton.WithCallbackData("Создать чат", MainRouteConstants.NEW)}
		};
		
		foreach (DocumentChat chat in model.DocChatList!) { 
			list.Add(new[] {
					InlineKeyboardButton.WithCallbackData(chat.ChatName, $"{MainRouteConstants.NAME}={chat.ChatName}"), 
					InlineKeyboardButton.WithCallbackData("Удалить", $"{MainRouteConstants.DELETE}={chat.ChatName}")
			});
		}
		InlineKeyboardMarkup markup = new(list);
		await _bot.SendTextMessageAsync(_message.Chat.Id, "<b>Выберите или создайте чат для ответы на вопросы по документам</b>", replyMarkup: markup);

	}
}