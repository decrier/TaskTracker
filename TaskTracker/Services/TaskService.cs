using TaskTracker.Models;
using TaskTracker.Repositories;

namespace TaskTracker.Services;

public class TaskService
{
    private readonly ITaskRepository _repository;
    private List<TaskItem> tasks;
    private int nextId = 1;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
        tasks = _repository.GetAllTasks();

        if (tasks.Count > 0)
        {
            nextId = tasks.Max(t => t.Id) + 1;
        }
    }

    public List<TaskItem> GetAllTasks()
    {
        return _repository.GetAllTasks();
    }

    public string AddTask(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "Task title should not be empty.";
        
        TaskItem newTask = new TaskItem
        {
            Id = nextId++,
            Title = title,
            IsDone = false,
            CreatedAt = DateTime.Now
        };
        
        _repository.AddTask(newTask);
        return $"Task added: {title}.";
    }

    public string MarkTaskAsDone(int id)
    {
        TaskItem? task = tasks.FirstOrDefault(t => t.Id == id);
        
        if (task == null)
        {
            return $"Task with id {id} not found.";
        }

        task.IsDone = true;
        _repository.UpdateTask(task);
        return $"Task {id} is done.";
    }

    public string UpdateTaskTitle(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return "Task title should not be empty.";

        TaskItem? task = _repository.GetTaskById(id);
        if (task is not null)
        {
            task.Title = title;
            _repository.UpdateTask(task);
            return $"Task updated: {title}.";
        }
        return $"Task with id {id} not found.";
    }

    public string DeleteTask(int id)
    {
        // TaskItem task = tasks.FirstOrDefault(t => t.Id == id);
        // if (task == null)
        // {
        //     return $"Task with id {id} not found.";
        // }
    
        // tasks.Remove(task);
        _repository.DeleteTask(id);
        return "Task deleted";
    }

    public List<TaskItem> GetTasksByFilter(TaskFilter filter)
    {
        tasks = _repository.GetAllTasks();
        return filter switch
        {
            TaskFilter.All => tasks,
            TaskFilter.Completed => tasks.Where(t => t.IsDone).ToList(),
            TaskFilter.NotCompleted => tasks.Where(t => !t.IsDone).ToList(),
            _ => new List<TaskItem>()
        };
    }


    public List<TaskItem> GetSortedTasks(TaskSortOption sortOption)
    {
        tasks = _repository.GetAllTasks();
        return sortOption switch
        {
            TaskSortOption.ById => tasks.OrderBy(t => t.Id).ToList(),
            TaskSortOption.ByTitle => tasks.OrderBy(t => t.Title).ToList(),
            TaskSortOption.ByCreatedAt => tasks.OrderBy(t => t.CreatedAt).ToList(),
            _ => new List<TaskItem>()
        };
    }

    public List<TaskItem> SearchTasks(string keyword)
    {
        tasks = _repository.GetAllTasks();
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<TaskItem>();

        return tasks
            .Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}