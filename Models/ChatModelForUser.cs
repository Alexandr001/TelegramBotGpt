using Models.KindOfChats;

namespace Models;

public class ChatModelForUser : IModel
{
	public long Id { get; set; }
	public Route Route { get; set; } = new();
	public List<DocumentChat> DocChatList { get; set; } = new();
	public List<TextChat> ChatList { get; set; } = new();

	public static T GetChatByList<T>(List<T> list, string chatName)
			where T : IChat
	{
		T first = list.First(c => c.Name == chatName);
		return first;
	}
}