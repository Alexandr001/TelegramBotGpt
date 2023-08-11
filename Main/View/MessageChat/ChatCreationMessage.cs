using IoC;
using Models;
using Models.KindOfChats;
using Repository;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;
using Route = Models.Route;

namespace Main.View.MessageChat;

public class ChatCreationMessage : IMessage
{
	private readonly TelegramBotClient _bot;
	private readonly IAwsRepository _awsRepository;
	private readonly IChatRepository<TextChat> _chatRepository;
	private readonly IChatRepository<DocumentChat> _docRepository;
	private readonly IUserRepository _userRepository;

	public ChatCreationMessage()
	{
		_bot = IoCContainer.GetService<TelegramBotClient>();
		_awsRepository = IoCContainer.GetService<IAwsRepository>();
		_chatRepository = IoCContainer.GetService<IChatRepository<TextChat>>();
		_docRepository = IoCContainer.GetService<IChatRepository<DocumentChat>>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}

	public async Task ChatMessageHandler(ChatModelForUser? model, Message message)
	{
		if (model.Route.ChatType != null) {
			if (model.Route.ChatRoute != MainRouteConstants.NEW) {
				throw new CustomException("Напиши $ еред сообщением!");
			}
			model.Route = new Route() {
					ChatType = MainRouteConstants.CHAT,
					ChatRoute = MainRouteConstants.NAME,
					ChatParam = message.Text
			};
			await _userRepository.EditUserRoute(model);
			await _chatRepository.AddChat(new TextChat() {
					Name = message.Text ?? ""
			}, message.Chat.Id);
			await _bot.SendTextMessageAsync(message.Chat.Id, "Чат создан!");
		}
	}

	public async Task DocMessageHandler(ChatModelForUser? model, Message message)
	{
		if (model.Route.ChatType != null) {
			if (model.Route.ChatRoute != MainRouteConstants.NEW) {
				throw new CustomException("Напиши $ перед сообщением!");
			}
			if (message.Caption?.Length > 60) {
				throw new CustomException("Слишком длинное название для чата!");
			}
			model.Route = new Route() {
					ChatType = MainRouteConstants.DOC,
					ChatRoute = MainRouteConstants.NAME,
					ChatParam = message.Caption
			};
			await _userRepository.EditUserRoute(model);
			await _docRepository.AddChat(new DocumentChat() {
					Name = message.Caption!,
					FileName = message.Document!.FileName!
			}, message.Chat.Id);
			await AddFileToAws(message);
			await _bot.SendTextMessageAsync(message.Chat.Id, "Чат создан! Документ добавлен!");
		}
	}

	private async Task AddFileToAws(Message message)
	{
		File fileAsync = await _bot.GetFileAsync(message.Document!.FileId);
		using MemoryStream stream = new();
		await _bot.DownloadFileAsync(fileAsync.FilePath!, stream);
		await _awsRepository.CreateFile(stream, message.Document.FileName!, message.Chat.Id.ToString());
	}
}