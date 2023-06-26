using Models;
using Models.KindOfChats;
using Repository;
using Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.MessageChat;

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
		GptService service = new();
		string response = await service.AskQuestionAsync(_message.Text!);
		await _bot.SendTextMessageAsync(_message.Chat.Id, "Продолжение чата по чату! Роут" + model.Route + "\n" + response);
	}

	public async Task DocMessageHandler(ChatModelForUser model)
	{
		if (model.ChatType == null) {
			throw new ArgumentNullException(nameof(model.ChatType), "Не выбран тип чата!");
		}
		DocumentChat documentChat = model.DocChatList!.First(d => d.ChatName == model.Route.ChatParam);
		HttpRequestService service = new();
		ResponceModel? responceModel = await service.GetResponce(_message.Text![1..], documentChat.FileName);

		await _bot.SendTextMessageAsync(_message.Chat.Id, responceModel!.Message);
	}
}