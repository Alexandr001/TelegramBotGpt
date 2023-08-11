using IoC;
using Models;
using Models.KindOfChats;
using Repository;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Callback;

public class DeletionCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly IAwsRepository _awsRepository;
	private readonly IChatRepository<TextChat> _chatRepository;
	private readonly IChatRepository<DocumentChat> _docRepository;

	public DeletionCallback()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_awsRepository = IoCContainer.GetService<IAwsRepository>();
		_chatRepository = IoCContainer.GetService<IChatRepository<TextChat>>();
		_docRepository = IoCContainer.GetService<IChatRepository<DocumentChat>>();
	}
	
	public async Task ChatCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		TextChat textChat =
				model.ChatList.First(o => o.Name == model.Route.ChatParam);
		model.ChatList.Remove(textChat);
		await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, "Чат удалён!");
		await _chatRepository.DeleteChat(textChat.Name);
	}

	public async Task DocCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery)
	{
		DocumentChat documentChat =
				model.DocChatList.First(o => o.Name == model.Route.ChatParam);
		model.DocChatList.Remove(documentChat);
		await _chatRepository.DeleteChat(documentChat.Name);
		await _awsRepository.DeleteFile(documentChat.Name, callbackQuery.Message!.Chat.Id.ToString());
		await _bot.AnswerCallbackQueryAsync(callbackQuery.Id, "Чат удалён!");
	}
}