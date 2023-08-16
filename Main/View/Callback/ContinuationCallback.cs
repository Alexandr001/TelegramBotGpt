using IoC;
using Models;
using Models.KindOfChats;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Callback;

public class ContinuationCallback : ICallback
{
	private const int MAX_LENGTH_MESSAGE = 4000;
	
	private readonly TelegramBotClient _bot;
	private readonly IChatRepository<TextChat> _textChatRepository;
	private readonly IChatRepository<DocumentChat> _docChatRepository;
	private readonly IUserRepository _userRepository;

	public ContinuationCallback()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_textChatRepository = IoCContainer.GetService<IChatRepository<TextChat>>();
		_docChatRepository = IoCContainer.GetService<IChatRepository<DocumentChat>>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}

	public async Task ChatCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = callbackQuery.Data;
		await _userRepository.EditUserRoute(model);
		TextChat chat = await _textChatRepository.GetChatHistory(model.Id, int.Parse(model.Route.ChatParam!));
		string stringHistory = History.HistoryListToString(chat.ChatHistory);
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!,
		                                callbackQuery.Message!.MessageId, 
		                                $"История сообщений чата \"{model.Route.ChatParam}\":\n" + stringHistory[^MAX_LENGTH_MESSAGE..]);
	}

	public async Task DocCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		model.Route = callbackQuery.Data;
		await _bot.EditMessageTextAsync(callbackQuery.Message?.Chat.Id!, 
		                                callbackQuery.Message!.MessageId, 
		                                "Тут будет история сообщений doc:");
	}
}