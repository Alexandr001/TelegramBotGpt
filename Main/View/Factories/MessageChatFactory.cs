using Main.View.MessageChat;
using Repository;
using Repository.Db.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Main.View.Factories;

public class MessageChatFactory : IFactory<char, IMessage>
{
	public IReadOnlyDictionary<char, IMessage> RouteAndObject { get; }

	public MessageChatFactory()
	{
		RouteAndObject = new Dictionary<char, IMessage>() {
				['$'] = new ChatContinuationMessage(),
				[default /*Значение по умолчанию*/] = new ChatCreationMessage(),
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