using IoC;
using Models;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Callback;

public class CreationCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly IUserRepository _userRepository;

	public CreationCallback()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}
	public async Task ChatCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = callbackQuery.Data;
		await _userRepository.EditUserRoute(model);
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!, 
		                                callbackQuery.Message!.MessageId, 
		                                "Введите название чата:");
	}

	public async Task DocCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = callbackQuery.Data;
		await _userRepository.EditUserRoute(model);
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!, 
		                                callbackQuery.Message!.MessageId, 
		                                "Введите название чата и прикрепите документ:");
	}
}