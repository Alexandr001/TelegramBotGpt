using Main.View.Chat;
using Main.View.Chat.ChatMessage;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.Controllers;

[ApiController]
[Route("/")]
public class BotController : ControllerBase
{
	private readonly TelegramBotClient _bot = BotModel.GetTelegramBot();
	private readonly ILogger<BotController> _logger;
	private readonly RedisRepository _repository;

	public BotController(ILogger<BotController> logger, RedisRepository repository)
	{
		_logger = logger;
		_repository = repository;
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
		ChatCallbackView callbackView = new(_bot, callback);
		if (callback.Message == null) {
			throw new NullReferenceException();
		}
		ChatModelForUser modelForUserById = await _repository.GetModelById<ChatModelForUser>(callback.Message!.Chat.Id) 
		                                    ?? throw new NullReferenceException();
		
		
		await callbackView.Create(callback.Data!, modelForUserById);
		await _repository.SetModel(modelForUserById);

	}
	
	private async Task MessageHandler(Message message)
	{
		IChatMessageView messageView;
		ChatModelForUser? modelById = await _repository.GetModelById<ChatModelForUser>(message.Chat.Id);
		if (modelById != null && modelById.Route.EndsWith("/new")) {
			messageView = new ChatMessageForCallback(_bot, message);
			await messageView.Handler(message.Text!, modelById);
			await _repository.SetModel(modelById);
		}

		if (message.Text?[0] == '/') {
			messageView = new ChatMessageForRoute(_bot, message);
			if (modelById == null) {
				modelById = await InitChatModelForUser(message);
			}
			await messageView.Handler(message.Text, modelById!);
		}
		
		if (message.Text?[0] == '$') {
			messageView = new ChatMessageForQuestions(_bot, message);
			await messageView.Handler(message.Text[1..], modelById!);
		}
		await _repository.SetModel(modelById!);
	}

	private async Task<ChatModelForUser> InitChatModelForUser(Message message)
	{
		await _repository.SetModel(new ChatModelForUser() {
				Id = message.Chat.Id
		});
		return (await _repository.GetModelById<ChatModelForUser>(message.Chat.Id))!;
	}
}