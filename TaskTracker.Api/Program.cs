using TaskTracker.Api.Configurations;
using TaskTracker.Api.Repositories;
using TaskTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings> (
    builder.Configuration.GetSection("DatabaseSettings"));

// Add services to the container.
builder.Services.AddLogging();
builder.Services.AddSingleton<ITaskRepository, SqliteTaskRepository>();
builder.Services.AddSingleton<TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();

    if (repository is SqliteTaskRepository sqliteTaskRepository)
    {
        await sqliteTaskRepository.InitializeDatabaseAsync();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();