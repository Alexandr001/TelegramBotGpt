namespace Models.KindOfChats;

public interface IChat
{
	public string Name { get; set; }
	public History? ChatHistory { get; set; }
}