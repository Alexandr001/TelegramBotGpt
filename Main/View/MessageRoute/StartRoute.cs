using IoC;
using Models;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.View.MessageRoute;

public class StartRoute : IRoute
{
	private readonly IUserRepository _userRepository;
	private readonly TelegramBotClient _bot;
	public StartRoute()
	{
		_userRepository = IoCContainer.GetService<IUserRepository>();
		_bot = IoCContainer.GetService<TelegramBotClient>();
	}
	public async Task RouteHandler(ChatModelForUser? model, Message message)
	{
		if (model is null) {
			model = new ChatModelForUser {
					Id = message.Chat.Id,
					Route = new Route() {
							ChatType = MainRouteConstants.START
					}
			};
			await _userRepository.CreateUser(model);
		}
		await _bot.SendTextMessageAsync(message.Chat.Id, "Тут будет приветственный текст.");
	}
}