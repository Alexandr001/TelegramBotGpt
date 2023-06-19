namespace Repository;

public class AwsRepository : IAwsRepository
{
	public Task<List<string>> GetListFiles(string directoryName)
	{
		throw new NotImplementedException();
	}

	public Task CreateFile(byte[] file, string name, string directoryName)
	{
		throw new NotImplementedException();
	}

	public Task DeleteFile(string name, string directoryName)
	{
		throw new NotImplementedException();
	}
}