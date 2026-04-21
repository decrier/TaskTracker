using TaskTracker.Models;
using TaskTracker.Repositories;

namespace TaskTracker.Tests;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = new();
    
    public Task <List<TaskItem>> GetAllTasksAsync()  => Task.FromResult(_tasks);

    public Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        TaskItem? task = _tasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task);
    }

    public Task AddTaskAsync(TaskItem task)
    {
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task UpdateTaskAsync(TaskItem task)
    {
        TaskItem? existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);

        if (existingTask is not null)
        {
            existingTask.Title = task.Title;
            existingTask.CreatedAt =  task.CreatedAt;
            existingTask.IsDone = task.IsDone;
        }
        return Task.CompletedTask;
    }

    public Task DeleteTaskAsync(int id)
    {
        TaskItem? task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task is not null)
        {
            _tasks.Remove(task);
        }
        
        return Task.CompletedTask;
    }
}