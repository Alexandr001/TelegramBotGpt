using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Chat.ChatMessage;

public class ChatMessageForQuestions : IChatMessageView
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public ChatMessageForQuestions(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}

	public async Task HandlerChat(string message, ChatModelForUser chatModelForUser)
	{
		// обращение к чату 
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Ты написал вот это? — " + message + "Такой роут: " + chatModelForUser.Route);
	}

	public Task HandlerDock(string message, ChatModelForUser chatModelForUser)
	{
		// обращение к сервису работы с доками
		throw new NotImplementedException();
	}
}