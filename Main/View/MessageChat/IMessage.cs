using Models;

namespace Main.View.MessageChat;

public interface IMessage
{
	Task ChatMessageHandler(ChatModelForUser model);
	Task DocMessageHandler(ChatModelForUser model);
}