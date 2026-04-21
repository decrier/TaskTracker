using TaskTracker.Models;

namespace TaskTracker.Repositories;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task AddTaskAsync(TaskItem task);
    Task UpdateTaskAsync(TaskItem task);
    Task DeleteTaskAsync(int id);
}