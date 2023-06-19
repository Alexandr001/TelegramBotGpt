using Models;
using Models.KindOfChats;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Main.View.Chat.ChatMessage;

public class ChatMessageForRoute : IChatMessageView
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;

	public ChatMessageForRoute(TelegramBotClient bot, Message message)
	{
		_bot = bot;
		_message = message;
	}
	public async Task Handler(string path, ChatModelForUser chatModelForUser)
	{
		switch (path) {
			case MainRouteConstants.DOC:
				await DocHandler(chatModelForUser);
				break;
			case MainRouteConstants.CHAT:
				await ChatHandler(chatModelForUser);
				break;
			case MainRouteConstants.INFO:
				await InfoHandler(chatModelForUser);
				break;
		}
	}

	private async Task DocHandler(ChatModelForUser chatModelForUser)
	{
		chatModelForUser.Route = MainRouteConstants.DOC;
		List<InlineKeyboardButton[]> list = new() {
				new[] {InlineKeyboardButton.WithCallbackData("Создать чат", nameof(ChatCallbackView.CreateChat))}
		};
		
		foreach (DocumentChat chat in chatModelForUser.DocChatList!) { 
			list.Add(new[] {
					InlineKeyboardButton.WithCallbackData(chat.ChatName, chat.ChatName), 
					InlineKeyboardButton.WithCallbackData("Удалить", $"delete={chat.ChatName}")
			});
		}
		InlineKeyboardMarkup markup = new(list);
		await _bot.SendTextMessageAsync(_message.Chat.Id, "<b>Выберите или создайте чат для ответы на вопросы по документам</b>", replyMarkup: markup);
	}
	
	private async Task ChatHandler(ChatModelForUser chatModelForUser)
	{
		chatModelForUser.Route = MainRouteConstants.CHAT;
		List<InlineKeyboardButton[]> list = new() {
				new[] {InlineKeyboardButton.WithCallbackData("Создать чат", nameof(ChatCallbackView.CreateChat))}
		};
		
		foreach (TextChat chat in chatModelForUser.ChatList!) { 
			list.Add(new[] {
					InlineKeyboardButton.WithCallbackData(chat.ChatName, chat.ChatName), 
					InlineKeyboardButton.WithCallbackData("Удалить", $"delete={chat.ChatName}")
			});
		}
		
		InlineKeyboardMarkup markup = new(list);
		await _bot.SendTextMessageAsync(_message.Chat.Id, "<b>Выберите или создайте чат для общения с ChatGPT</b>", replyMarkup: markup);
	}

	private async Task InfoHandler(ChatModelForUser chatModelForUser)
	{
		chatModelForUser.Route = MainRouteConstants.INFO;
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Тут будет текст описания бота!");
	}

	public static string GetNameByRoute(string s)
	{
		return s[(s.IndexOf('=') + 1)..];
	}
}