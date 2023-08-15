namespace Models.KindOfChats;

public class DocumentChat : IChat
{
	public string Name { get; set; } = string.Empty;
	public List<History> ChatHistory { get; set; } = new();
	public string FileName { get; set; } = string.Empty;
}