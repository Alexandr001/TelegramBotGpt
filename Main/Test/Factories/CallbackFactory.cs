using Main.Test.Callback;
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
				["new"] = new CreationCallback(bot, callbackQuery, awsRepository),
				["delete"] = new DeletionCallback(bot, callbackQuery, awsRepository),
				["name"] = new ContinuationCallback(bot, callbackQuery, awsRepository)
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