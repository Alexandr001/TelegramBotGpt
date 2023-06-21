using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.Test.MessageChat;

public class ChatContinuationMessage : IMessage
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public ChatContinuationMessage(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}
	public async Task ChatMessageHandler(ChatModelForUser model)
	{
		if (model.ChatType == null) {
			throw new ArgumentNullException(nameof(model.ChatType), "Не выбран тип чата!");
		}
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Продолжение чата по чату! Роут" + model.Route);

	}

	public async Task DocMessageHandler(ChatModelForUser model)
	{
		if (model.ChatType == null) {
			throw new ArgumentNullException(nameof(model.ChatType), "Не выбран тип чата!");
		}
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Продолжение чата по документу! Роут" + model.Route);
	}
}