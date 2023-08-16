namespace Repository.Db.Interfaces;

public interface IChatRepository<T>
		where T : class
{
	Task<T> GetChatHistory(long userId, int chatId);
	Task<int> AddChat(T model, long chatId);
	Task AddHistory(T chat);
	Task DeleteChat(int id);
}