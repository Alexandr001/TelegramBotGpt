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
		var dbModel = new {
				UserId = chatId, 
				ChatName = model.Name, 
				FileName = model.FileName
		};

		const string SQL_INSERT_CHAT = "INSERT INTO DocChat (Name, userId, FileName) VALUE " 
		                               + $"(@{nameof(dbModel.ChatName)}, "
		                               + $"@{nameof(dbModel.UserId)}, "
		                               + $"{nameof(dbModel.FileName)});";

		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(SQL_INSERT_CHAT, dbModel);
	}

	public async Task<DocumentChat> GetChatHistory(long userId, string chatName)
	{
		const string SQL_QUERY = "SELECT DC.Name, DC.FileName, HD.UserMessage, HD.BotMessage FROM DocChat DC " 
		                         + "INNER JOIN HistoryDoc HD "
		                         + "on DC.Name = HD.Name " 
		                         + "WHERE DC.Name = @Name AND DC.userId = @UserId " 
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
		string sqlQuery = "DELETE FROM DocChat WHERE Name = @Name";

		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(sqlQuery, new {Name = name});
	}

	public async Task AddHistory(DocumentChat chatName)
	{
		var model = new {
				ChatName = "",
				UserMessage = "",
				BotMessage = "",
				Index = 0
		};
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryDoc (Name, UserMessage, BotMessage, sequenceNumber) " + "VALUES("
		                                  + $"@{nameof(model.ChatName)}, " 
		                                  + $"@{nameof(model.UserMessage)}, " 
		                                  + $"@{nameof(model.BotMessage)}, "
		                                  + $"@{nameof(model.Index)})";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction transaction = connection.BeginTransaction();
		for (int i = 0; i < chatName.ChatHistory!.UserMessages.Count; i++) {
			model = new {
					ChatName = chatName.Name, 
					UserMessage = chatName.ChatHistory!.UserMessages[i], 
					BotMessage = chatName.ChatHistory!.BotMessages[i], 
					Index = i};
			await connection.ExecuteAsync(SQL_INSERT_HISTORY, model, transaction);
		}
		transaction.Commit();
	}
}