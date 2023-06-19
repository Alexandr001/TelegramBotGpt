namespace Repository;

public interface IAwsRepository
{
	Task<List<string>> GetListFiles(string directoryName);
	Task CreateFile(byte[] file, string name, string directoryName);
	Task DeleteFile(string name, string directoryName);
}