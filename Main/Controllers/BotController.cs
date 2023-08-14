using IoC;
using Main.View.Callback;
using Main.View.Factories;
using Main.View.MessageChat;
using Main.View.MessageRoute;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Route = Models.Route;

namespace Main.Controllers;

[ApiController]
[Route("/")]
public class BotController : ControllerBase
{
	private readonly TelegramBotClient _bot = BotModel.GetTelegramBot();
	private readonly ILogger<BotController> _logger;
	private readonly IRedisRepository _redisRepository;
	private readonly IUserRepository _userRepository;

	public BotController(ILogger<BotController> logger)
	{
		_logger = logger;
		_redisRepository = IoCContainer.GetService<IRedisRepository>();
		_userRepository = IoCContainer.GetService<IUserRepository>();
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		return Ok("<h1>Hello! I'm BOT!</h1>");
	}

	[HttpPost]
	public async Task<IActionResult> Post(Update update)
	{
		try {
			if (update.CallbackQuery != null) {
				await CallbackHandler(update.CallbackQuery);
				return Ok();
			}
			if (update.Message != null) {
				await MessageHandler(update.Message);
				return Ok();
			}
		} catch (CustomException ce) {
			if (update.Message?.Chat.Id != null) {
				await _bot.SendTextMessageAsync(update.Message.Chat.Id, ce.Message);
			} else {
				await _bot.SendTextMessageAsync(update.CallbackQuery!.Message!.Chat.Id, ce.Message);
			}
		} catch (Exception e) {
			string errorMes = e.ToString();
			_logger.LogError(errorMes);
			if (update.Message?.Chat.Id != null) {
				await _bot.SendTextMessageAsync(update.Message.Chat.Id, errorMes);
			} else {
				await _bot.SendTextMessageAsync(update.CallbackQuery!.Message!.Chat.Id, errorMes);
			}
			return Ok();
		}
		return Ok();
	}

	private async Task CallbackHandler(CallbackQuery callback)
	{
		ChatModelForUser? model = await _userRepository.GetUser(callback.Message.Chat.Id);
		model.Route = callback.Data;
		IFactory<string, ICallback> factory = new CallbackFactory();
		ICallback? factoryMethod = factory.FactoryMethod(model.Route.ChatRoute!);
		if (factoryMethod == null) {
			throw new CustomException("Фабрика не создалась!");
		}
		if (model.Route.ChatType == MainRouteConstants.CHAT) {
			await factoryMethod.ChatCallbackHandler(model, callback);
		}
		if (model.Route.ChatType == MainRouteConstants.DOC) {
			await factoryMethod.DocCallbackHandler(model, callback);
		}
	}

	private async Task MessageHandler(Message message)
	{
		if (message.Text != null && message.Text[0] == '/') {
			await RouteHandler(message);
		} else {
			await ChatHandler(message);
		}
	}

	private async Task RouteHandler(Message message)
	{
		ChatModelForUser? model = await _userRepository.GetUser(message.Chat.Id);
		IFactory<string, IRoute> factory = new RouteFactory();
		IRoute? factoryMethod = factory.FactoryMethod(message.Text![1..]);
		if (factoryMethod == null) {
			throw new CustomException("Фабрика не создалась! Неверно введённый роут");
		}
		await factoryMethod.RouteHandler(model, message);
	}

	private async Task ChatHandler(Message message)
	{
		string? text = message.Text ?? message.Caption;
		if (text == null) {
			throw new CustomException("Неверно введенное сообщение!");
		}
		ChatModelForUser? model = await _userRepository.GetUser(message.Chat.Id);
		IFactory<char, IMessage> factory = new MessageChatFactory();
		IMessage? messageFactory = factory.FactoryMethod(text);
		if (messageFactory == null) {
			throw new CustomException("Фабрика не создалась! Что то с сообщениями");
		}
		if (model.Route.ChatType == MainRouteConstants.CHAT) {
			await messageFactory.ChatMessageHandler(model, message);
		}
		if (model.Route.ChatType == MainRouteConstants.DOC) {
			await messageFactory.DocMessageHandler(model, message);
		}
	}

	// private async Task<ChatModelForUser> GetChatModelForUser(long id)
	// {
	// 	ChatModelForUser? chatModelForUser = await _redisRepository.GetModelById<ChatModelForUser>(id);
	// 	if (chatModelForUser == null) {
	// 		await _redisRepository.SetModel(new ChatModelForUser() {
	// 				Id = id
	// 		});
	// 		chatModelForUser = await _redisRepository.GetModelById<ChatModelForUser>(id);
	// 	}
	// 	return chatModelForUser!;
	// }
}