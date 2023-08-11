using Models;
using Telegram.Bot.Types;

namespace Main.View.MessageRoute;

public interface IRoute
{
	Task RouteHandler(ChatModelForUser model, Message message);
}