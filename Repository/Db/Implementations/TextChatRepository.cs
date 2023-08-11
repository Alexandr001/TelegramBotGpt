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

	public async Task AddChat(TextChat model, long chatId)
	{
		(long UserId, string ChatName) dbModel;

		const string SQL_INSERT_CHAT = "INSERT INTO TextChat (name, userId) VALUE " + $"(@{nameof(dbModel.ChatName)}, @{nameof(dbModel.UserId)});";

		using IDbConnection connection = _context.Connection();
		using IDbTransaction transaction = connection.BeginTransaction();
		dbModel = (chatId, model.ChatName);
		await connection.ExecuteAsync(SQL_INSERT_CHAT, dbModel);
		transaction.Commit();
	}

	public async Task<TextChat> GetChatHistory(long userId, string chatName)
	{
		const string SQL_QUERY = "SELECT TC.name, HT.UserMessage, HT.BotMessage FROM TextChat TC " + "INNER JOIN HistoryText HT "
		                         + "on TC.name = HT.ChatName " + "WHERE TC.name = @ChatName AND TC.userId = @UserId " + "ORDER BY HT.sequenceNumber;";
		using IDbConnection connection = _context.Connection();
		IEnumerable<TextChat> queryAsync = await connection.QueryAsync<TextChat, History, TextChat>(SQL_QUERY, (chat, history) => {
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
		string sqlQuery = "DELETE FROM TextChat WHERE name = @name";

		using IDbConnection connection = _context.Connection();
		using IDbTransaction tran = connection.BeginTransaction();
		await connection.ExecuteAsync(sqlQuery, name);
		tran.Commit();
	}

	public async Task AddHistory(TextChat chatName)
	{
		(string ChatName, string UserMessage, string BotMessage, int Index) model;
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryText (ChatName, UserMessage, BotMessage, sequenceNumber) " + "VALUES("
		                                  + $"@{nameof(model.ChatName)}, " + $"@{nameof(model.UserMessage)}, " + $"@{nameof(model.BotMessage)}, "
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