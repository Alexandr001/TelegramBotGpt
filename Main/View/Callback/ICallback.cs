using Models;
using Telegram.Bot.Types;

namespace Main.View.Callback;

public interface ICallback
{
	Task ChatCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery);
	Task DocCallbackHandler(ChatModelForUser? model, CallbackQuery callbackQuery);
}