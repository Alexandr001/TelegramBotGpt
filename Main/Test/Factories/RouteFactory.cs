using Main.Test.MessageRoute;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.Test.Factories;

public class RouteFactory : IFactory<string, IRoute>
{
	public IReadOnlyDictionary<string, IRoute> RouteAndObject { get; }

	public RouteFactory(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		RouteAndObject = new Dictionary<string, IRoute>() {
				[MainRouteConstants.CHAT] = new ChatRoute(bot, message, awsRepository),
				[MainRouteConstants.DOC] = new DocRoute(bot, message, awsRepository),
				[MainRouteConstants.INFO] = new InfoRoute(bot, message, awsRepository)
		};
	}

	public IRoute? FactoryMethod(string route)
	{
		foreach ((string? key, IRoute? value) in RouteAndObject) {
			if (route == key) {
				return value;
			}
		}
		return null;
	}
}