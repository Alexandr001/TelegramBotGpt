namespace Models.KindOfChats;

public class TextChat : IChat
{
	public string ChatName { get; set; } = string.Empty;
	public History? ChatHistory { get; set; }
}