using TaskTracker.Models;

namespace TaskTracker.Repositories;

public interface ITaskRepository
{
    List<TaskItem> GetAllTasks();
    TaskItem? GetTaskById(int id);
    void AddTask(TaskItem task);
    void UpdateTask(TaskItem task);
    void DeleteTask(int id);
}