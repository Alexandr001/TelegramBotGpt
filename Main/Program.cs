using Main.Core;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Di.Register(builder);
// IConfigurationSection sectionAws = builder.Configuration.GetSection("AWS");
// AwsAccessModel? awsAccessModel = builder.Configuration.GetSection("AWS").Get<AwsAccessModel>();

//builder.Services.AddTransient<TelegramBotClient>(_ => new TelegramBotClient(builder.Configuration.GetSection("BotKey").Value!));

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

