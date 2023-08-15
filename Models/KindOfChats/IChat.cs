namespace Models.KindOfChats;

public interface IChat
{
	public string Name { get; set; }
	public List<History> ChatHistory { get; set; }
}