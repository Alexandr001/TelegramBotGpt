using IoC;
using Models;
using Models.KindOfChats;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Route = Models.Route;

namespace Main.View.MessageRoute;


public class ChatRoute : IRoute
{
	private readonly TelegramBotClient _bot;
	private readonly IUserRepository _userRepository;

	public ChatRoute()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}
	public async Task RouteHandler(ChatModelForUser model, Message message)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.CHAT
		};
		await _userRepository.EditUserRoute(model);
		List<InlineKeyboardButton[]> list = new() {
				new[] {InlineKeyboardButton.WithCallbackData("Создать чат", MainRouteConstants.NEW)}
		};

		foreach (TextChat chat in model.ChatList!) {
			list.Add(new[] {
					InlineKeyboardButton.WithCallbackData(chat.Name, $"{MainRouteConstants.NAME}={chat.Name}"),
					InlineKeyboardButton.WithCallbackData("`Удалить`", $"{MainRouteConstants.DELETE}={chat.Name}")
			});
		}
		InlineKeyboardMarkup markup = new(list);
		await _bot.SendTextMessageAsync(message.Chat.Id, "*Выберите или создайте чат для общения с ChatGPT*", replyMarkup: markup);
	}
}