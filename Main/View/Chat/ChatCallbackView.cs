using Main.View.Chat.ChatMessage;
using Models;
using Models.KindOfChats;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Chat;

public class ChatCallbackView
{
	private readonly TelegramBotClient _bot;
	private readonly CallbackQuery _callbackQuery;

	public ChatCallbackView(TelegramBotClient bot, CallbackQuery callbackQuery)
	{
		_bot = bot;
		_callbackQuery = callbackQuery;
	}

	public async Task Create(string name, ChatModelForUser chatModelForUser)
	{
		if (name == nameof(CreateChat)) {
			await CreateChat(chatModelForUser);
		} else if (IsContinueChat(name, chatModelForUser)) {
			await ContinueChat(chatModelForUser);
		} else if (IsContinueChat(ChatMessageForRoute.GetNameByRoute(name), chatModelForUser)) {
			await DeleteChat(chatModelForUser);
		}
	}
	public async Task CreateChat(ChatModelForUser chatModelForUser)
	{
		// ToDo: Захоркоденый док
		chatModelForUser.Route = "/doc/new";
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, 
		                                _callbackQuery.Message!.MessageId, 
		                                "Введите название чата:");
	}

	public async Task ContinueChat(ChatModelForUser chatModelForUser)
	{
		// ToDo: Захоркоденый док
		chatModelForUser.Route = $"/doc/name={_callbackQuery.Data}";
		// Получить историю чатов
		await _bot.EditMessageTextAsync(_callbackQuery.Message?.Chat.Id!, 
		                                _callbackQuery.Message!.MessageId, 
		                                "Тут будет история сообщений:");
	}

	public async Task DeleteChat(ChatModelForUser chatModelForUser)
	{
		DocumentChat documentChat = chatModelForUser.DocChatList!.First(o => 
				                                                                o.ChatName == ChatMessageForRoute.GetNameByRoute(_callbackQuery.Data!));
		chatModelForUser.DocChatList!.Remove(documentChat);
		await _bot.AnswerCallbackQueryAsync(_callbackQuery.Id, "Чат удалён!");
	}

	public bool IsContinueChat(string chatName, ChatModelForUser modelForUser)
	{
		foreach (DocumentChat documentChat in modelForUser.DocChatList!) {
			if (documentChat.ChatName == chatName) {
				return true;
			}
		}
		return false;
	}
}