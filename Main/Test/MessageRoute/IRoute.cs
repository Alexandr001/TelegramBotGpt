using Models;

namespace Main.Test.MessageRoute;

public interface IRoute
{
	Task RouteHandler(ChatModelForUser model);
}