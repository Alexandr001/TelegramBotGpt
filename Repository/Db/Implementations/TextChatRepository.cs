using System.Data;
using Dapper;
using IoC;
using Models;
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
		var dbModel = new {
				UserId = chatId,
				ChatName = model.Name
		};
		const string SQL_INSERT_CHAT = "INSERT INTO TextChat (Name, userId) VALUE " + 
		                               $"(@{nameof(dbModel.ChatName)}, @{nameof(dbModel.UserId)});";

		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(SQL_INSERT_CHAT, dbModel);
	}

	public async Task<TextChat> GetChatHistory(long userId, string chatName)
	{
		const string SQL_QUERY = "SELECT TC.Name, HT.UserMessage, HT.BotMessage FROM TextChat TC " + 
		                         "LEFT JOIN HistoryText HT " + 
		                         "on TC.Name = HT.Name " + 
		                         "WHERE TC.Name = @Name AND TC.userId = @UserId " + 
		                         "ORDER BY HT.sequenceNumber;";
		using IDbConnection connection = _context.Connection();
		IEnumerable<TextChat?> queryAsync = await connection.QueryAsync<TextChat, History, TextChat>(
		                                     SQL_QUERY, 
		                                     (chat, history) => {
			                                     chat.ChatHistory.Add(history);
			                                     return chat;
		                                     }, new {
				                                     Name = chatName,
				                                     UserId = userId
		                                     }, splitOn: "UserMessage");
		IEnumerable<TextChat> textChats = queryAsync.GroupBy(t => t.Name)
		                                            .ToList()
		                                            .Select(g => {
			                                            TextChat? groupedChat = g.First();
			                                            groupedChat.ChatHistory = g.Select(p => p.ChatHistory.Single())
			                                                                       .ToList();
			                                            return groupedChat;
		                                            });
		return textChats.First();
	}

	public async Task DeleteChat(string name)
	{
		string sqlQuery = "DELETE FROM TextChat WHERE Name = @Name";

		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(sqlQuery, new {Name = name});
	}

	public async Task AddHistory(TextChat chatName)
	{
		var model = new {
				ChatName = "",
				UserMessage = "",
				BotMessage = "",
				Index = 0
		};
		const string SQL_INSERT_HISTORY = "INSERT INTO HistoryText (Name, UserMessage, BotMessage, sequenceNumber) " + 
		                                  "VALUES(" + 
		                                  $"@{nameof(model.ChatName)}, " + 
		                                  $"@{nameof(model.UserMessage)}, " + 
		                                  $"@{nameof(model.BotMessage)}, " + 
		                                  $"@{nameof(model.Index)})";

		using IDbConnection connection = _context.Connection();
		connection.Open();
		using IDbTransaction transaction = connection.BeginTransaction();
		for (int i = 0; i < chatName.ChatHistory.Count; i++) {
			model = new {
					ChatName = chatName.Name, 
					UserMessage = chatName.ChatHistory[i].UserMessages, 
					BotMessage = chatName.ChatHistory[i].BotMessages, 
					Index = i};
			await connection.ExecuteAsync(SQL_INSERT_HISTORY, model, transaction);
		}
		transaction.Commit();
	}
}