using Main.Test.Callback;
using Main.Test.Factories;
using Main.Test.MessageChat;
using Main.Test.MessageRoute;
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
			return NotFound();
		} catch (Exception e) {
			_logger.LogError(e.ToString());
			return BadRequest();
		}
	}

	private async Task CallbackHandler(CallbackQuery callback)
	{
		ChatModelForUser model = await GetChatModelForUser(callback.Message!.Chat.Id);
		Route route = Route.Parse(str: callback.Data);
		model.Route = route;
		IFactory<string, ICallback> factory = new CallbackFactory(_bot, callback, _awsRepository);
		ICallback? factoryMethod = factory.FactoryMethod(route.ChatRoute!);
		if (factoryMethod == null) {
			throw new ArgumentNullException(nameof(factoryMethod), "Callback handler");
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
		if (message.Text[0] == '/') {
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
			throw new ArgumentNullException(nameof(factoryMethod), "RoutrHandler");
		}
		await factoryMethod.RouteHandler(model);
		await _repository.SetModel(model);
	}

	private async Task ChatHandler(Message message)
	{
		ChatModelForUser model = await GetChatModelForUser(message.Chat.Id);
		IFactory<char, IMessage> factory = new MessageChatFactory(_bot, message, _awsRepository);
		IMessage? messageFactory = factory.FactoryMethod(message.Text!);
		if (messageFactory == null) {
			throw new ArgumentNullException(nameof(messageFactory), "ChatHandler");
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