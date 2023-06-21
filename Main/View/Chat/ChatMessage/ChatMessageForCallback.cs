using Models;
using Models.KindOfChats;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;
using Route = Models.Route;

namespace Main.View.Chat.ChatMessage;

public class ChatMessageForCallback : IChatMessageView
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public ChatMessageForCallback(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}

	public async Task HandlerChat(string message, ChatModelForUser chatModelForUser)
	{
		chatModelForUser.Route = new Route() {
				ChatType = MainRouteConstants.CHAT,
				ChatRoute = "name",
				ChatParam = message
		};

		chatModelForUser.ChatList!.Add(new TextChat() {
				ChatName = _message.Text!
		});
	}

	public async Task HandlerDock(string message, ChatModelForUser chatModelForUser)
	{
		chatModelForUser.Route = new Route() {
				ChatType = MainRouteConstants.DOC,
				ChatRoute = "name",
				ChatParam = message
		};
		chatModelForUser.DocChatList!.Add(new DocumentChat() {
				ChatName = _message.Text!
		});
		await AddFileToAws();
	}

	private async Task AddFileToAws()
	{
		File fileAsync = await _bot.GetFileAsync(_message.Document!.FileId);
		using MemoryStream stream = new();
		await _bot.DownloadFileAsync(fileAsync.FilePath!, stream);
		await _awsRepository.CreateFile(stream, _message.Document.FileName!, "");
	}
}