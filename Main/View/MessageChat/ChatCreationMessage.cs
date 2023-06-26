﻿using Models;
using Models.KindOfChats;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;
using Route = Models.Route;

namespace Main.View.MessageChat;

public class ChatCreationMessage : IMessage
{
	private readonly TelegramBotClient _bot;
	private readonly Message _message;
	private readonly IAwsRepository _awsRepository;

	public ChatCreationMessage(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		_bot = bot;
		_message = message;
		_awsRepository = awsRepository;
	}

	public async Task ChatMessageHandler(ChatModelForUser model)
	{
		if (model.ChatType != null) {
			if (model.Route.ChatRoute != MainRouteConstants.NEW) {
				throw new CustomException("Напиши $ еред сообщением!");
			}
			model.Route = new Route() {
					ChatType = MainRouteConstants.CHAT,
					ChatRoute = MainRouteConstants.NAME,
					ChatParam = _message.Text
			};
			model.ChatList!.Add(new TextChat() {
					ChatName = _message.Text!
			});
		}
	}

	public async Task DocMessageHandler(ChatModelForUser model)
	{
		if (model.ChatType != null) {
			if (model.Route.ChatRoute != MainRouteConstants.NEW) {
				throw new CustomException("Напиши $ перед сообщением!");
			}
			if (_message.Caption?.Length > 60) {
				
			}
			model.Route = new Route() {
					ChatType = MainRouteConstants.DOC,
					ChatRoute = MainRouteConstants.NAME,
					ChatParam = _message.Caption
			};
			model.DocChatList!.Add(new DocumentChat() {
					ChatName = _message.Caption!,
					FileName = _message.Document!.FileName!
			});
			await AddFileToAws();
		}
	}

	private async Task AddFileToAws()
	{
		File fileAsync = await _bot.GetFileAsync(_message.Document!.FileId);
		using MemoryStream stream = new();
		await _bot.DownloadFileAsync(fileAsync.FilePath!, stream);
		await _awsRepository.CreateFile(stream, _message.Document.FileName!, "");
	}
}