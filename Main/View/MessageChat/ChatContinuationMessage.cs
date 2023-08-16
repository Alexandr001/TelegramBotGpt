using IoC;
using Models;
using Models.KindOfChats;
using Repository.Db.Interfaces;
using Service;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.MessageChat;

public class ChatContinuationMessage : IMessage
{
	private readonly TelegramBotClient _bot;
	private readonly IGptService _service;
	private readonly IChatRepository<DocumentChat> _docRepository;
	private readonly IChatRepository<TextChat> _chatRepository;

	public ChatContinuationMessage()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_service = IoCContainer.GetService<IGptService>();
		_chatRepository = IoCContainer.GetService<IChatRepository<TextChat>>();
		_docRepository = IoCContainer.GetService<IChatRepository<DocumentChat>>();
	}

	public async Task ChatMessageHandler(ChatModelForUser? model, Message message)
	{
		if (model.Route.ChatType == null) {
			throw new ArgumentNullException(nameof(model.Route.ChatType), "Не выбран тип чата!");
		}
		TextChat chatHistory = await _chatRepository.GetChatHistory(model.Id, int.Parse(model.Route.ChatParam!));
		string response = await _service.AskQuestionAsync(chatHistory.ChatHistory, message.Text!);
		await _chatRepository.AddHistory(new TextChat() {
				Id = chatHistory.Id,
				Name = chatHistory.Name,
				ChatHistory = new List<History>() {
						new() {
								UserMessage = message.Text,
								BotMessage = response
						}
				}
		});
		
		await _bot.SendTextMessageAsync(message.Chat.Id, "Продолжение чата по чату! Роут" + model.Route + "\n" + response);
	}

	public async Task DocMessageHandler(ChatModelForUser? model, Message message)
	{
		
		if (model.Route.ChatType == null) {
			throw new ArgumentNullException(nameof(model.Route.ChatType), "Не выбран тип чата!");
		}
		DocumentChat documentChat = model.DocChatList!.First(d => d.Name == model.Route.ChatParam);
		HttpService service = new();
		ResponceModel? responceModel = await service.GetResponce(message.Text![1..], documentChat.FileName);

		await _bot.SendTextMessageAsync(message.Chat.Id, responceModel!.Message);
	}
}