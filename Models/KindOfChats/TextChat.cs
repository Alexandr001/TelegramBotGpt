using System.Text;

namespace Models.KindOfChats;

public class TextChat : IChat
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<History> ChatHistory { get; set; } = new();
}