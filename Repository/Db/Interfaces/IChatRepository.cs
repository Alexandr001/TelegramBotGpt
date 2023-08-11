namespace Repository.Db.Interfaces;

public interface IChatRepository<T>
		where T : class
{
	Task<T> GetChatHistory(long userId, string chatName);
	Task AddChat(T model, long chatId);
	Task AddHistory(T chat);
	Task DeleteChat(string name);
}