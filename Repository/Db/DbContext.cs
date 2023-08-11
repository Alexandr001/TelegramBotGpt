using IoC;
using Models;
using MySql.Data.MySqlClient;

namespace Repository.Db;

public class DbContext
{
	private readonly string _connectionString;

	public DbContext()
	{
		_connectionString = IoCContainer.GetConstant<string>(Constants.SQL_CONNECTION);
	}
	public MySqlConnection Connection() => new(_connectionString);
}