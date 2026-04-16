using Microsoft.Data.Sqlite;
using TaskTracker.Models;

namespace TaskTracker.Repositories;

public class SqliteTaskRepository : ITaskRepository
{
    private const string ConnectionString = "Data Source=tasks.db";

    public SqliteTaskRepository()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                         CREATE TABLE IF NOT EXISTS Tasks (
                             Id INTEGER PRIMARY KEY,
                             Title TEXT NOT NULL,
                             IsDone INTEGER NOT NULL,
                             CreatedAt TEXT NOT NULL
                             );
                         """;
            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database initialization error", ex);
        }
    }

    public List<TaskItem> GetAllTasks()
    {
        try
        {
            List<TaskItem> tasks = new List<TaskItem>();
        
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = "SELECT Id, Title, IsDone, CreatedAt FROM Tasks";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
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
        
            return tasks;
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database read error", ex);
        }
        catch (FormatException ex)
        {
            throw new Exception("Date format error in database: " + ex.Message);
        }
    }

    public TaskItem? GetTaskById(int id)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = "SELECT * FROM tasks WHERE Id = @id;";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    IsDone = reader.GetInt32(2) == 1,
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                };
            }
            return null;
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database read error", ex);
        }
        catch (FormatException ex)
        {
            throw new Exception("Date format error in database: " + ex.Message);
        }
    }

    public void AddTask(TaskItem task)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string insertSql = """
                               INSERT INTO Tasks (Id, Title, IsDone, CreatedAt)
                               VALUES (@id, @title, @isDone, @createdAt);
                               """;

            using var insertCommand = new SqliteCommand(insertSql, connection);
            insertCommand.Parameters.AddWithValue("@id", task.Id);
            insertCommand.Parameters.AddWithValue("@title", task.Title);
            insertCommand.Parameters.AddWithValue("@isDone", task.IsDone ? 1 : 0);
            insertCommand.Parameters.AddWithValue("@createdAt", task.CreatedAt.ToString("O"));
            
            insertCommand.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database insert error", ex);
        }
    }

    public void UpdateTask(TaskItem task)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = """
                         UPDATE Tasks 
                         SET Title = @title,
                             IsDone = @isDone,
                             CreatedAt = @createdAt
                         WHERE Id = @id;
                         """;

            using var updateCommand = new SqliteCommand(sql, connection);
            updateCommand.Parameters.AddWithValue("@id", task.Id);
            updateCommand.Parameters.AddWithValue("@title", task.Title);
            updateCommand.Parameters.AddWithValue("@isDone", task.IsDone ? 1 : 0);
            updateCommand.Parameters.AddWithValue("@createdAt", DateTime.Parse(task.CreatedAt.ToString("O")));

            updateCommand.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database update error", ex);
        }
    }

    public void DeleteTask(int id)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            string sql = "DELETE FROM Tasks WHERE Id = @id;";

            using var deleteCommand = new SqliteCommand(sql, connection);
            deleteCommand.Parameters.AddWithValue("@id", id);
            deleteCommand.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new Exception("Database delete error", ex);
        }
    }
}