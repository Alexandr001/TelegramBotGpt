using Models.KindOfChats;

namespace Models;

public class ChatModelForUser : IModel
{
	private List<DocumentChat> _docChatList = new();
	private List<TextChat> _chatList = new();
	public long Id { get; set; }
	public Route Route { get; set; } = new();
	public List<DocumentChat> DocChatList
	{
		get { return _docChatList; }
		set { _docChatList = value ?? new List<DocumentChat>(); }
	}
	public List<TextChat> ChatList
	{
		get { return _chatList; }
		set { _chatList = value ?? new List<TextChat>(); }
	}

	public static T GetChatByList<T>(List<T> list, string chatName)
			where T : IChat
	{
		T first = list.First(c => c.ChatName == chatName);
		return first;
	}
}