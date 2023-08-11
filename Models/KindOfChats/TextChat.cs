namespace Models.KindOfChats;

public class TextChat : IChat
{
	public string Name { get; set; } = string.Empty;
	public History? ChatHistory { get; set; }
}