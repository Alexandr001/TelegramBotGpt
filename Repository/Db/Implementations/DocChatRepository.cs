using System.Data;
using Dapper;
using IoC;
using Models.KindOfChats;
using Repository.Db.Interfaces;

namespace Repository.Db.Implementations;

public class DocChatRepository : IChatRepository<DocumentChat>
{
	private readonly DbContext _context;

	public DocChatRepository()
	{
		_context = IoCContainer.GetService<DbContext>();
	}
	public async Task<int> AddChat(DocumentChat model, long chatId)
	{
		var dbModel = new {
				UserId = chatId, 
				ChatName = model.Name, 
				FileName = model.FileName
		};

		const string SQL_INSERT_CHAT = "INSERT INTO DocChat (Name, userId, FileName) VALUE " 
		                               + $"(@{nameof(dbModel.ChatName)}, "
		                               + $"@{nameof(dbModel.UserId)}, "
		                               + $"@{nameof(dbModel.FileName)});" + 
		                               "SELECT LAST_INSERT_ID()";

		using IDbConnection connection = _context.Connection();
		int id = await connection.QueryFirstOrDefaultAsync<int>(SQL_INSERT_CHAT, dbModel);
		return id;
	}

	public async Task<DocumentChat> GetChatHistory(long userId, int chatId)
	{
		const string SQL_QUERY = "SELECT DC.id, DC.name, DC.fileName, HD.userMessage, HD.botMessage FROM DocChat DC " + 
		                         "LEFT JOIN HistoryDoc HD " + 
		                         "on DC.id = HD.chatId " + 
		                         "WHERE DC.id = @ChatId AND DC.userId = @UserId;";
		using IDbConnection connection = _context.Connection();
		IEnumerable<DocumentChat> queryAsync = await connection.QueryAsync<DocumentChat, History, DocumentChat>(SQL_QUERY, (chat, history) => {
			chat.ChatHistory.Add(history);
			return chat;
		}, new {
				ChatId = chatId,
				UserId = userId
		});
		return queryAsync.First();
	}

	public async Task DeleteChat(int id)
	{
		const string FIRS_QUERY = "DELETE FROM HistoryDoc WHERE chatId = @Id";
		const string SECOND_QUERY = "DELETE FROM DocChat WHERE id = @Id;";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction beginTransaction = connection.BeginTransaction();
		await connection.ExecuteAsync(FIRS_QUERY, new {Id = id});
		await connection.ExecuteAsync(SECOND_QUERY, new {Id = id});
		beginTransaction.Commit();
	}

	public async Task AddHistory(DocumentChat chat)
	{
		var model = new {
				ChatId = 0,
				UserMessage = "",
				BotMessage = ""
		};
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryDoc (chatId, userMessage, botMessage) " + "VALUES(" + 
		                                  $"@{nameof(model.ChatId)}, " + 
		                                  $"@{nameof(model.UserMessage)}, " + 
		                                  $"@{nameof(model.BotMessage)};";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction transaction = connection.BeginTransaction();
		for (int i = 0; i < chat.ChatHistory.Count; i++) {
			model = new {
					ChatId = chat.Id,
					UserMessage = chat.ChatHistory[i].UserMessage,
					BotMessage = chat.ChatHistory![i].BotMessage};
			await connection.ExecuteAsync(SQL_INSERT_HISTORY, model, transaction);
		}
		transaction.Commit();
	}
}