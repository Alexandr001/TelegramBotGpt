using Models;

namespace Main.View.MessageRoute;

public interface IRoute
{
	Task RouteHandler(ChatModelForUser model);
}