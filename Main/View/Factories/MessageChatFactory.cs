using Main.Test.Factories;
using Main.View.MessageChat;
using Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Factories;

public class MessageChatFactory : IFactory<char, IMessage>
{
	public IReadOnlyDictionary<char, IMessage> RouteAndObject { get; }

	public MessageChatFactory(TelegramBotClient bot, Message message, IAwsRepository awsRepository)
	{
		RouteAndObject = new Dictionary<char, IMessage>() {
				['$'] = new ChatContinuationMessage(bot, message, awsRepository),
				[default /*Значение по умолчанию*/] = new ChatCreationMessage(bot, message, awsRepository),
		};
	}
	public IMessage FactoryMethod(string message)
	{
		foreach ((char key, IMessage? value) in RouteAndObject) {
			if (message[0] == key) {
				return value;
			}
		}
		return RouteAndObject[default];
	}
}