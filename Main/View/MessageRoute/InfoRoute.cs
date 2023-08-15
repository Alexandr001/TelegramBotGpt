using IoC;
using Models;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.View.MessageRoute;

public class InfoRoute : IRoute
{
	private readonly TelegramBotClient _bot;
	private readonly IUserRepository _userRepository;

	public InfoRoute()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}
	public async Task RouteHandler(ChatModelForUser model, Message message)
	{
		model.Route = new Route() {
				ChatType = MainRouteConstants.INFO
		};
		await _userRepository.EditUserRoute(model);
		await _bot.SendTextMessageAsync(message.Chat.Id, "Тут будет текст описания бота!");
	}
}