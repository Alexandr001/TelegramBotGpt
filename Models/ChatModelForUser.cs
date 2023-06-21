using Models.KindOfChats;

namespace Models;

public class ChatModelForUser : IModel
{
	private List<DocumentChat> _docChatList = new();
	private List<TextChat> _chatList = new();
	public long Id { get; set; }
	public string? ChatType { get; set; }
	public Route Route { get; set; } = new();
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

public class Route
{
	public string ChatType { get; set; } = string.Empty;
	public string? ChatRoute { get; set; }
	public string? ChatParam { get; set; }
	
	public override string ToString()
	{
		if (ChatParam == null) {
			if (ChatRoute == null) {
				return '/' + ChatType;
			}
			return '/' + ChatType + '/' + ChatRoute;
		}
		return '/' + ChatType + '/' + ChatRoute + '=' + ChatParam;
	}
}