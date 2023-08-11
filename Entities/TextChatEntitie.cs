using System.Text;

namespace Entities;

public interface ITextChat
{
	void CreateChatText();
	string ContinueChatText();
	void DeleteChatText();
}

public interface IDocumentChat
{
	void CreateChatDoc();
	string ContinueChatDoc();
	void DeleteChatDoc();
}

public class TextChat
{
	
}
public class DocChat
{
	public string ChatName { get; set; } = string.Empty;
	public DocumentModel? Document { get; set; }

	public History? history { get; set; }
}

public class History
{
	public List<string> UserMessages { get; set; } = new();
	public List<string> BotMessages { get; set; } = new();

	public override string ToString()
	{
		StringBuilder sb = new();
		if (UserMessages.Count != BotMessages.Count) {
			return "История чатов сломалась :(";
		}
		for (int i = UserMessages.Count - 1; i >= 0; i--) {
			sb.Append("Ваше сообщение:\n" + UserMessages[i]);
			sb.Append("Сообщение бота:\n" + BotMessages[i]);	
		}
		return sb.ToString();
	}
}

public class DocumentModel
{
	public string FileName { get; set; } = string.Empty;
	public MemoryStream File { get; set; } = null!;
}