namespace Models.KindOfChats;

public interface IChat
{
	public int Id { get; set; }
	public string Name { get; set; }
	public List<History> ChatHistory { get; set; }
}