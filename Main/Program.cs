using Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options => {
	options.Configuration = "localhost";
	options.InstanceName = "local";
});
builder.Services.AddTransient<RedisRepository>();
builder.Services.AddTransient<IAwsRepository, AwsRepository>(_ => new AwsRepository(
                                                                                    builder.Configuration.GetSection("serviceUrl").Value!,
                                                                                    builder.Configuration.GetSection("accessKey").Value!,
                                                                                    builder.Configuration.GetSection("secretKey").Value!,
                                                                                    builder.Configuration.GetSection("bucketName").Value!
                                                                                   ));
// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();