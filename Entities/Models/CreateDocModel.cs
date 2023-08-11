namespace Entities.Models;

public class CreateDocModel
{
	public string Name { get; set; }
	public string FileName { get; set; }
	public MemoryStream File { get; set; }
}