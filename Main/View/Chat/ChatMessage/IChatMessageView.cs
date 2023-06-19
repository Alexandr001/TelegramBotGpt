using Models;
using Models.KindOfChats;

namespace Main.View.Chat.ChatMessage;

public interface IChatMessageView
{
	Task Handler(string message, ChatModelForUser chatModelForUser);
}