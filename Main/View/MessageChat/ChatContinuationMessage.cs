using IoC;
using Models;
using Models.KindOfChats;
using Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.MessageChat;

public class ChatContinuationMessage : IMessage
{
	private readonly TelegramBotClient _bot;
	private readonly IGptService _service;

	public ChatContinuationMessage()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_service = IoCContainer.GetService<IGptService>();
	}

	public async Task ChatMessageHandler(ChatModelForUser? model, Message message)
	{
		//ToDo: Получить историю и добавить её в сообщение. Добавить новое сообщение с ответом в бд.
		if (model.Route.ChatType == null) {
			throw new ArgumentNullException(nameof(model.Route.ChatType), "Не выбран тип чата!");
		}
		History? chatHistory = ChatModelForUser.GetChatByList(model.ChatList, model.Route.ChatParam!).ChatHistory;
		string response = await _service.AskQuestionAsync(chatHistory, message.Text!);
		await _bot.SendTextMessageAsync(message.Chat.Id, "Продолжение чата по чату! Роут" + model.Route + "\n" + response);
	}

	public async Task DocMessageHandler(ChatModelForUser? model, Message message)
	{
		//ToDo: Получить историю и добавить её в сообщение. Добавить новое сообщение с ответом в бд.

		if (model.Route.ChatType == null) {
			throw new ArgumentNullException(nameof(model.Route.ChatType), "Не выбран тип чата!");
		}
		DocumentChat documentChat = model.DocChatList!.First(d => d.ChatName == model.Route.ChatParam);
		HttpService service = new();
		ResponceModel? responceModel = await service.GetResponce(message.Text![1..], documentChat.FileName);

		await _bot.SendTextMessageAsync(message.Chat.Id, responceModel!.Message);
	}
}