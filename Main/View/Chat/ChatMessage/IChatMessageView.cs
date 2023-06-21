using Models;
using Models.KindOfChats;

namespace Main.View.Chat.ChatMessage;

public interface IChatMessageView
{
	Task HandlerChat(string message, ChatModelForUser chatModelForUser);
	Task HandlerDock(string message, ChatModelForUser chatModelForUser);
}