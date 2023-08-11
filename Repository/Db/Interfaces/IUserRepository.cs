using Models;

namespace Repository.Db.Interfaces;

public interface IUserRepository
{
	public Task<ChatModelForUser?> GetUser(long id);
	public Task CreateUser(ChatModelForUser model);
	public Task EditUserRoute(ChatModelForUser model);
}