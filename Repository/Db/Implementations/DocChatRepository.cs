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
	public async Task AddChat(DocumentChat model, long chatId)
	{
		(long UserId, string ChatName, string FileName) dbModel;

		const string SQL_INSERT_CHAT = "INSERT INTO DocChat (name, userId, fileName) VALUE " 
		                               + $"(@{nameof(dbModel.ChatName)}, "
		                               + $"@{nameof(dbModel.UserId)}, "
		                               + $"{nameof(dbModel.FileName)});";

		using IDbConnection connection = _context.Connection();
		using IDbTransaction transaction = connection.BeginTransaction();
		dbModel = (chatId, model.ChatName, model.FileName);
		await connection.ExecuteAsync(SQL_INSERT_CHAT, dbModel);
		transaction.Commit();
	}

	public async Task<DocumentChat> GetChatHistory(long userId, string chatName)
	{
		const string SQL_QUERY = "SELECT DC.name, DC.fileName, HD.UserMessage, HD.BotMessage FROM DocChat DC " 
		                         + "INNER JOIN HistoryDoc HD "
		                         + "on DC.name = HD.ChatName " 
		                         + "WHERE DC.name = @ChatName AND DC.userId = @UserId " 
		                         + "ORDER BY HD.sequenceNumber;";
		using IDbConnection connection = _context.Connection();
		IEnumerable<DocumentChat> queryAsync = await connection.QueryAsync<DocumentChat, History, DocumentChat>(SQL_QUERY, (chat, history) => {
			chat.ChatHistory = history;
			return chat;
		}, new {
				ChatHame = chatName,
				UserId = userId
		});
		return queryAsync.First();
	}

	public async Task DeleteChat(string name)
	{
		string sqlQuery = "DELETE FROM DocChat WHERE name = @name";

		using IDbConnection connection = _context.Connection();
		using IDbTransaction tran = connection.BeginTransaction();
		await connection.ExecuteAsync(sqlQuery, name);
		tran.Commit();
	}

	public async Task AddHistory(DocumentChat chatName)
	{
		(string ChatName, string UserMessage, string BotMessage, int Index) model;
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryDoc (ChatName, UserMessage, BotMessage, sequenceNumber) " + "VALUES("
		                                  + $"@{nameof(model.ChatName)}, " 
		                                  + $"@{nameof(model.UserMessage)}, " 
		                                  + $"@{nameof(model.BotMessage)}, "
		                                  + $"@{nameof(model.Index)})";

		using IDbConnection connection = _context.Connection();
		using IDbTransaction transaction = connection.BeginTransaction();
		for (int i = 0; i < chatName.ChatHistory!.UserMessages.Count; i++) {
			model = (chatName.ChatName, chatName.ChatHistory!.UserMessages[i], chatName.ChatHistory!.BotMessages[i], i);
			await connection.ExecuteAsync(SQL_INSERT_HISTORY, model, transaction);
		}
		transaction.Commit();
	}
}