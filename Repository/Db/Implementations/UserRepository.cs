using System.Data;
using Dapper;
using IoC;
using Models;
using Models.KindOfChats;
using Repository.Db.Interfaces;

namespace Repository.Db.Implementations;

public class UserRepository : IUserRepository
{
	private readonly DbContext _context;

	public UserRepository()
	{
		_context = IoCContainer.GetService<DbContext>();
	}

	public async Task<ChatModelForUser?> GetUser(long id)
	{
		string sqlQuery = "SELECT U.id, U.route, TC.name, DC.Name " + 
		                  "FROM User as U " + 
		                  "LEFT JOIN TextChat TC on U.id = TC.userId " + 
		                  "LEFT JOIN DocChat DC on U.id = DC.UserId " + 
		                  "WHERE U.id = @Id";
		using IDbConnection connection = _context.Connection();
		IEnumerable<ChatModelForUser?> chatModelForUsers = 
				await connection.QueryAsync<ChatModelForUser, TextChat, DocumentChat, ChatModelForUser>(
				 sqlQuery,
				 (user, chatT, chatD) => {
					 user.ChatList.Add(chatT);
					 user.DocChatList.Add(chatD);
					 return user;
				 },
				 new {Id = id}, splitOn: "Name");
		if (!chatModelForUsers.Any()) {
			return null;
		}
		List<ChatModelForUser> modelForUsers = chatModelForUsers.GroupBy(m => m.Id)
		                                                        .ToList()
		                                                        .Select(g => {
			                                                        ChatModelForUser? groupedChat = g.First();
			                                                        groupedChat.ChatList = g.Select(p => p.ChatList.Single())
			                                                                                .ToList();
			                                                        groupedChat.DocChatList = g.Select(p => p.DocChatList.Single())
			                                                                                   .ToList();
			                                                        return groupedChat;
		                                                        }).ToList();
		return chatModelForUsers.First();
	}

	public async Task CreateUser(ChatModelForUser model)
	{
		string sqlQuery = "INSERT INTO User (id, route) VALUES (@Id, @Route);";
		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(sqlQuery, new {
				Id = model.Id,
				Route = model.Route.ToString()
		});
	}

	public async Task EditUserRoute(ChatModelForUser model)
	{
		string sqlQuery = "UPDATE User SET route = @Route WHERE id = @Id;";

		using IDbConnection connection = _context.Connection();
		await connection.ExecuteAsync(sqlQuery, new {
				Id = model.Id,
				Route = model.Route.ToString()
		});
	}
}