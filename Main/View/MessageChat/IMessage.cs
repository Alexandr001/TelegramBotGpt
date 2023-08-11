using Models;
using Telegram.Bot.Types;

namespace Main.View.MessageChat;

public interface IMessage
{
	Task ChatMessageHandler(ChatModelForUser? model, Message message);
	Task DocMessageHandler(ChatModelForUser? model, Message message);
}