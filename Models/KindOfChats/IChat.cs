namespace Models.KindOfChats;

public interface IChat
{
	public string ChatName { get; set; }
	public History? ChatHistory { get; set; }
}