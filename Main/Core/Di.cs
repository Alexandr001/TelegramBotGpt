using IoC;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Models;
using Models.KindOfChats;
using Repository;
using Repository.Db;
using Repository.Db.Implementations;
using Repository.Db.Interfaces;
using Service;
using Telegram.Bot;

namespace Main.Core;

public static class Di
{
	public static void Register(WebApplicationBuilder builder)
	{
		IoCContainer.RegisterConstants(new Dictionary<string, object>() {
				[Constants.REDIS_CACHE] = new RedisCache(new RedisCacheOptions {
						Configuration = "localhost",
						InstanceName = "local"
				}),
				[Constants.AWS_ACCESS] = builder.Configuration.GetSection("AWS").Get<AwsAccessModel>()!,
				[Constants.SQL_CONNECTION] = builder.Configuration.GetConnectionString("sqlConnection")!,
				[Constants.BOT_KEY] = builder.Configuration.GetSection("BotKey").Value!,
				[Constants.GPT_KEY] = builder.Configuration.GetSection("GptKey").Value!
		
		});
		
		IoCContainer.RegisterService<DbContext>();
		IoCContainer.RegisterService<IAwsRepository, AwsRepository>();
		IoCContainer.RegisterService<IRedisRepository, RedisRepository>();
		IoCContainer.RegisterService<IChatRepository<DocumentChat>, DocChatRepository>();
		IoCContainer.RegisterService<IChatRepository<TextChat>, TextChatRepository>();
		IoCContainer.RegisterService<IUserRepository, UserRepository>();
		IoCContainer.RegisterService<IGptService, GptService>();
		IoCContainer.RegisterService<IHttpService, HttpService>();
		IoCContainer.RegisterService(new TelegramBotClient(IoCContainer.GetConstant<string>(Constants.BOT_KEY)));

	}
}