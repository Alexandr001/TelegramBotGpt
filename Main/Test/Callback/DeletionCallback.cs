using Models;
using Models.KindOfChats;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.Test.Callback;

public class DeletionCallback : ICallback
{
	private readonly TelegramBotClient _bot;
	private readonly CallbackQuery _callbackQuery;
	private readonly IAwsRepository _awsRepository;

	public DeletionCallback(TelegramBotClient bot, CallbackQuery callbackQuery, IAwsRepository awsRepository)
	{
		_bot = bot;
		_callbackQuery = callbackQuery;
		_awsRepository = awsRepository;
	}

	public async Task ChatCallbackHandler(ChatModelForUser model)
	{
		
		TextChat textChat =
				model.ChatList!.First(o => o.ChatName == model.Route.ChatParam);
		model.ChatList!.Remove(textChat);
		await _bot.AnswerCallbackQueryAsync(_callbackQuery.Id, "Чат удалён!");
			
	}

	public async Task DocCallbackHandler(ChatModelForUser model)
	{
		DocumentChat documentChat =
				model.DocChatList!.First(o => o.ChatName == model.Route.ChatParam);
		model.DocChatList!.Remove(documentChat);
		await _awsRepository.DeleteFile(documentChat.FileName, "");
		await _bot.AnswerCallbackQueryAsync(_callbackQuery.Id, "Чат doc удалён!");
	}
}