using Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Chat.ChatMessage;

public class ChatMessageForQuestions : IChatMessageView
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;

	public ChatMessageForQuestions(TelegramBotClient bot, Message message)
	{
		_bot = bot;
		_message = message;
	}
	public async Task Handler(string message, ChatModelForUser chatModelForUser)
	{
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Ты написал вот это? — " + message + "Такой роут: " + chatModelForUser.Route);
	}
}