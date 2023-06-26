using Main.Test.Callback;
using Main.View.Callback;
using Main.View.Factories;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.Test.Factories;

public class CallbackFactory : IFactory<string, ICallback>
{
	public IReadOnlyDictionary<string, ICallback> RouteAndObject { get; }

	public CallbackFactory(TelegramBotClient bot, CallbackQuery callbackQuery, IAwsRepository awsRepository)
	{
		RouteAndObject = new Dictionary<string, ICallback>() {
				[MainRouteConstants.NEW] = new CreationCallback(bot, callbackQuery, awsRepository),
				[MainRouteConstants.DELETE] = new DeletionCallback(bot, callbackQuery, awsRepository),
				[MainRouteConstants.NAME] = new ContinuationCallback(bot, callbackQuery, awsRepository)
		};
	}

	public ICallback? FactoryMethod(string str)
	{
		foreach ((string? key, ICallback? value) in RouteAndObject) {
			if (str == key) {
				return value;
			}
		}
		return null;
	}
}