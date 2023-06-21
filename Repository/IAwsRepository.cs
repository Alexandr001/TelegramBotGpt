namespace Repository;

public interface IAwsRepository
{
	Task CreateFile(Stream stream, string name, string directoryName);
	Task DeleteFile(string name, string directoryName);
}