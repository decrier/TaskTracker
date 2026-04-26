using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TaskTracker.Api.Configurations;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Repositories;

public class SqliteTaskRepository : ITaskRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqliteTaskRepository> _logger;

    public SqliteTaskRepository(
        IOptions<DatabaseSettings> options,
        ILogger<SqliteTaskRepository> logger)
    {
        _connectionString = options.Value.ConnectionString;
        _logger = logger;
        
        _logger.LogInformation("SqliteTaskRepository initialized with database source.");
    }

    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = """
                         CREATE TABLE IF NOT EXISTS Tasks (
                             Id INTEGER PRIMARY KEY,
                             Title TEXT NOT NULL,
                             IsDone INTEGER NOT NULL,
                             CreatedAt TEXT NOT NULL
                             );
                         """;
            await using var command = new SqliteCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
            
            _logger.LogInformation("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization error");
            throw;
        }
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        try
        {
            List<TaskItem> tasks = new ();
        
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT Id, Title, IsDone, CreatedAt FROM Tasks";

            await using var command = new SqliteCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                TaskItem taskItem = new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    IsDone = reader.GetInt32(2) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                };
                
                tasks.Add(taskItem);
            }
            _logger.LogInformation($"Loaded {tasks.Count} tasks from database.");
            return tasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while loading tasks from database");
            throw;
        }
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM tasks WHERE Id = @id;";

            await using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new TaskItem
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                IsDone = reader.GetInt32(2) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(3))
            };
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Error while searching a task");
            throw;
        }
    }

    public async Task AddTaskAsync(TaskItem task)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string insertSql = """
                               INSERT INTO Tasks (Id, Title, IsDone, CreatedAt)
                               VALUES (@id, @title, @isDone, @createdAt);
                               """;

            await using var insertCommand = new SqliteCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@id", task.Id);
            insertCommand.Parameters.AddWithValue("@title", task.Title);
            insertCommand.Parameters.AddWithValue("@isDone", task.IsDone ? 1 : 0);
            insertCommand.Parameters.AddWithValue("@createdAt", task.CreatedAt.ToString("O"));
            
            await insertCommand.ExecuteNonQueryAsync();
            _logger.LogInformation($"Task with id {task.Id} successfully added.");
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex,"Error while adding a task");
        }
    }

    public async Task UpdateTaskAsync(TaskItem task)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = """
                         UPDATE Tasks 
                         SET Title = @title,
                             IsDone = @isDone,
                             CreatedAt = @createdAt
                         WHERE Id = @id;
                         """;

            await using var updateCommand = new SqliteCommand(sql, connection);
            updateCommand.Parameters.AddWithValue("@id", task.Id);
            updateCommand.Parameters.AddWithValue("@title", task.Title);
            updateCommand.Parameters.AddWithValue("@isDone", task.IsDone ? 1 : 0);
            updateCommand.Parameters.AddWithValue("@createdAt", DateTime.Parse(task.CreatedAt.ToString("O")));

            await updateCommand.ExecuteNonQueryAsync();
            
            _logger.LogInformation($"Task wit id {task.Id} successfully updated");
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, $"Error while updating task with id {task.Id}", ex);
        }
    }

    public async Task DeleteTaskAsync(int id)
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM Tasks WHERE Id = @id;";

            await using var deleteCommand = new SqliteCommand(sql, connection);
            deleteCommand.Parameters.AddWithValue("@id", id);
            
            await deleteCommand.ExecuteNonQueryAsync();
            _logger.LogInformation($"Task with id {id} deleted.");
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, $"Error while deleting task with id {id}");
        }
    }
}