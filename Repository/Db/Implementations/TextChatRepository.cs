using System.Data;
using Dapper;
using IoC;
using Models.KindOfChats;
using Repository.Db.Interfaces;

namespace Repository.Db.Implementations;

public class TextChatRepository : IChatRepository<TextChat>
{
	private readonly DbContext _context;

	public TextChatRepository()
	{
		_context = IoCContainer.GetService<DbContext>();
	}

	public async Task<int> AddChat(TextChat model, long chatId)
	{
		var dbModel = new {
				UserId = chatId,
				ChatName = model.Name
		};
		const string SQL_INSERT_CHAT = "INSERT INTO TextChat (name, userId) VALUE " + 
		                               $"(@{nameof(dbModel.ChatName)}, @{nameof(dbModel.UserId)});" + 
		                               "SELECT LAST_INSERT_ID();";

		using IDbConnection connection = _context.Connection();
		int id = await connection.QueryFirstOrDefaultAsync<int>(SQL_INSERT_CHAT, dbModel);
		return id;
	}

	public async Task<TextChat> GetChatHistory(long userId, int chatId)
	{
		const string SQL_QUERY = "SELECT TC.id, TC.name, HT.userMessage, HT.botMessage FROM TextChat TC " + 
		                         "LEFT JOIN HistoryText HT " + 
		                         "on TC.id = HT.chatId " + 
		                         "WHERE TC.id = @Id AND TC.userId = @UserId";
		using IDbConnection connection = _context.Connection();
		IEnumerable<TextChat?> queryAsync = await connection.QueryAsync<TextChat, History, TextChat>(
		                                     SQL_QUERY, 
		                                     (chat, history) => {
			                                     chat.ChatHistory.Add(history);
			                                     return chat;
		                                     }, new {
				                                     Id = chatId,
				                                     UserId = userId
		                                     }, splitOn: "UserMessage");
		TextChat textChats = queryAsync.GroupBy(t => t.Id)
		                               .Select(g => {
			                               TextChat? groupedChat = g.First();
			                               groupedChat.ChatHistory = g.Select(p => p.ChatHistory.Single())
			                                                          .ToList();
			                               return groupedChat;
		                               }).Single();
		return textChats;
	}

	public async Task DeleteChat(int id)
	{
		const string FIRS_QUERY = "DELETE FROM HistoryText WHERE chatId = @Id";
		const string SECOND_QUERY = "DELETE FROM TextChat WHERE id = @Id;";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction beginTransaction = connection.BeginTransaction();
		await connection.ExecuteAsync(FIRS_QUERY, new {Id = id});
		await connection.ExecuteAsync(SECOND_QUERY, new {Id = id});
		beginTransaction.Commit();
	}

	public async Task AddHistory(TextChat chatName)
	{
		var model = new {
				Id = 0,
				UserMessage = "",
				BotMessage = ""
		};
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryText (chatId, userMessage, botMessage) " + "VALUES(" + 
		                                  $"@{nameof(model.Id)}, " + 
		                                  $"@{nameof(model.UserMessage)}, " + 
		                                  $"@{nameof(model.BotMessage)});";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction transaction = connection.BeginTransaction();
		for (int i = 0; i < chatName.ChatHistory.Count; i++) {
			model = new {
					Id = chatName.Id, 
					UserMessage = chatName.ChatHistory[i].UserMessage, 
					BotMessage = chatName.ChatHistory[i].BotMessage};
			await connection.ExecuteAsync(SQL_INSERT_HISTORY, model, transaction);
		}
		transaction.Commit();
	}
}