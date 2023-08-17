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
	private readonly IHttpService _httpService;

	public ChatContinuationMessage()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_service = IoCContainer.GetService<IGptService>();
		_chatRepository = IoCContainer.GetService<IChatRepository<TextChat>>();
		_docRepository = IoCContainer.GetService<IChatRepository<DocumentChat>>();
		_httpService = IoCContainer.GetService<IHttpService>();
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
		DocumentChat documentChat = model.DocChatList!.First(d => d.Id == int.Parse(model.Route.ChatParam!));
		ResponceModel? responceModel = await _httpService.GetResponce(message.Text![1..], $"{model.Id}/{documentChat.FileName}");

		await _bot.SendTextMessageAsync(message.Chat.Id, responceModel!.Message);
	}
}