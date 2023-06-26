using Main.Test.Callback;
using Main.Test.Factories;
using Main.View.Factories;
using Main.View.MessageChat;
using Main.View.MessageRoute;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
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
	private readonly RedisRepository _repository;
	private readonly IAwsRepository _awsRepository;

	public BotController(ILogger<BotController> logger, RedisRepository repository, IAwsRepository awsRepository)
	{
		_logger = logger;
		_repository = repository;
		_awsRepository = awsRepository;
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
		ChatModelForUser model = await GetChatModelForUser(callback.Message!.Chat.Id);
		Route route = Route.Parse(str: callback.Data);
		model.Route = route;
		IFactory<string, ICallback> factory = new CallbackFactory(_bot, callback, _awsRepository);
		ICallback? factoryMethod = factory.FactoryMethod(route.ChatRoute!);
		if (factoryMethod == null) {
			throw new CustomException("Фабрика не создалась!");
		}
		if (model.ChatType == MainRouteConstants.CHAT) {
			await factoryMethod.ChatCallbackHandler(model);
		}
		if (model.ChatType == MainRouteConstants.DOC) {
			await factoryMethod.DocCallbackHandler(model);
		}
		await _repository.SetModel(model);
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
		ChatModelForUser model = await GetChatModelForUser(message.Chat.Id);
		IFactory<string, IRoute> factory = new RouteFactory(_bot, message, _awsRepository);
		IRoute? factoryMethod = factory.FactoryMethod(message.Text![1..]);
		if (factoryMethod == null) {
			throw new CustomException("Фабрика не создалась! Неверно введённый роут");
		}
		await factoryMethod.RouteHandler(model);
		await _repository.SetModel(model);
	}

	private async Task ChatHandler(Message message)
	{
		string? text = message.Text ?? message.Caption;
		if (text == null) {
			throw new CustomException("Неверно введенное сообщение!");
		}
		ChatModelForUser model = await GetChatModelForUser(message.Chat.Id);
		IFactory<char, IMessage> factory = new MessageChatFactory(_bot, message, _awsRepository);
		IMessage? messageFactory = factory.FactoryMethod(text);
		if (messageFactory == null) {
			throw new CustomException("Фабрика не создалась! Что то с сообщениями");
		}
		if (model.ChatType == MainRouteConstants.CHAT) {
			await messageFactory.ChatMessageHandler(model);
		}
		if (model.ChatType == MainRouteConstants.DOC) {
			await messageFactory.DocMessageHandler(model);
		}
		await _repository.SetModel(model);
	}

	private async Task<ChatModelForUser> GetChatModelForUser(long id)
	{
		ChatModelForUser? chatModelForUser = await _repository.GetModelById<ChatModelForUser>(id);
		if (chatModelForUser == null) {
			await _repository.SetModel(new ChatModelForUser() {
					Id = id
			});
			chatModelForUser = await _repository.GetModelById<ChatModelForUser>(id);
		}
		return chatModelForUser!;
	}
}