namespace Models.KindOfChats;

public class DocumentChat : IChat
{
	public string Name { get; set; } = string.Empty;
	public History? ChatHistory { get; set; }
	public string FileName { get; set; } = string.Empty;
}