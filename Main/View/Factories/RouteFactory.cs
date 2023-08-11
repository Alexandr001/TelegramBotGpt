using Main.View.MessageRoute;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Factories;

public class RouteFactory : IFactory<string, IRoute>
{
	public IReadOnlyDictionary<string, IRoute> RouteAndObject { get; }

	public RouteFactory()
	{
		RouteAndObject = new Dictionary<string, IRoute>() {
				[MainRouteConstants.CHAT] = new ChatRoute(),
				[MainRouteConstants.DOC] = new DocRoute(),
				[MainRouteConstants.INFO] = new InfoRoute(),
				[MainRouteConstants.START] = new StartRoute()
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