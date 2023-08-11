namespace Models.KindOfChats;

public class DocumentChat : IChat
{
	public string ChatName { get; set; } = string.Empty;
	public History? ChatHistory { get; set; }
	public string FileName { get; set; } = string.Empty;
}