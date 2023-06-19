using Models.KindOfChats;

namespace Models;

public class ChatModelForUser : IModel
{
	private List<DocumentChat> _docChatList = new();
	private List<TextChat> _chatList = new();
	public long Id { get; set; }
	public string Route { get; set; } = string.Empty;
	public List<DocumentChat>? DocChatList
	{
		get { return _docChatList; }
		set { _docChatList = value ?? new List<DocumentChat>(); }
	}
	public List<TextChat>? ChatList
	{
		get { return _chatList; }
		set { _chatList = value ?? new List<TextChat>(); }
	}
	
	
}