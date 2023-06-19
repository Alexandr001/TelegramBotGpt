using Models;
using Models.KindOfChats;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Chat.ChatMessage;

public class ChatMessageForCallback : IChatMessageView
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;

	public ChatMessageForCallback(TelegramBotClient bot, Message message)
	{
		_bot = bot;
		_message = message;
	}

	public async Task Handler(string message, ChatModelForUser chatModelForUser)
	{
		// ToDo: Тут нужно зарефакторить закордхоженный "/doc"
		chatModelForUser.Route = $"/doc/name={message}";
		chatModelForUser.DocChatList!.Add(new DocumentChat() {
				ChatName = _message.Text!
		});
	}
}