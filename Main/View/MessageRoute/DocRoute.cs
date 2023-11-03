using IoC;
using Models;
using Models.KindOfChats;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Route = Models.Route;

namespace Main.View.MessageRoute;

public class DocRoute : IRoute
{
	private readonly TelegramBotClient _bot;
	private readonly IUserRepository _userRepository;

	public DocRoute()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}
	
	public async Task RouteHandler(ChatModelForUser model, Message message)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.DOC
		};
		await _userRepository.EditUserRoute(model);
		List<InlineKeyboardButton[]> list = new() {
				new[] {InlineKeyboardButton.WithCallbackData("Создать чат", $"/{MainRouteConstants.DOC}/{MainRouteConstants.NEW}")}
		};

		foreach (DocumentChat chat in model.DocChatList) {
			list.Add(new[] {
					InlineKeyboardButton.WithCallbackData(chat.Name, $"/{MainRouteConstants.DOC}/{MainRouteConstants.NAME}={chat.Id}"), 
					InlineKeyboardButton.WithCallbackData("Удалить", $"/{MainRouteConstants.DOC}/{MainRouteConstants.DELETE}={chat.Id}")
			});
		}
		InlineKeyboardMarkup markup = new(list);
		await _bot.SendTextMessageAsync(message.Chat.Id, "Выберите или создайте чат для ответы на вопросы по документам", replyMarkup: markup);
	}
}