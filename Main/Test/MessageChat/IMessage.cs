using Models;

namespace Main.Test.MessageChat;

public interface IMessage
{
	Task ChatMessageHandler(ChatModelForUser model);
	Task DocMessageHandler(ChatModelForUser model);
}