using TaskTracker.Api.Common;
using TaskTracker.Api.Models;
using TaskTracker.Api.Repositories;

namespace TaskTracker.Api.Services;

public class TaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskService> _logger;
    private List<TaskItem> tasks = new ();
    private int nextId = 1;

    public TaskService(
        ITaskRepository repository,
        ILogger<TaskService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        tasks = await _repository.GetAllTasksAsync();

        if (tasks.Count > 0)
        {
            nextId = tasks.Max(t => t.Id) + 1;
        }
        _logger.LogInformation($"TaskService initialized. NextId = {nextId}");
    }

    public async Task<List<TaskItem>> GetAllTasksAsync()
    {
        tasks = await _repository.GetAllTasksAsync();
        return tasks.ToList();
    }

    public async Task<OperationResult<TaskItem>> GetTaskByIdAsync(int id)
    {
        var task = await _repository.GetTaskByIdAsync(id);

        if (task is null)
        {
            _logger.LogWarning($"Task {id} not found");
            return OperationResult<TaskItem>.Fail($"Task with id {id} not found");
        }

        return OperationResult<TaskItem>.Ok(task);
    }

    public async Task<OperationResult> AddTaskAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Attempt to add task with empty title");
            return OperationResult.Fail("Title task should not be empty");
        }

        TaskItem newTask = new TaskItem
        {
            Id = nextId++,
            Title = title,
            IsDone = false,
            CreatedAt = DateTime.Now
        };
        
        await _repository.AddTaskAsync(newTask);
        
        _logger.LogInformation($"Task with id {newTask.Id} created.");
        return OperationResult.Ok($"Task added: {title}.");
    }

    public async Task<OperationResult> MarkTaskAsDoneAsync(int id)
    {
        TaskItem? task = tasks.FirstOrDefault(t => t.Id == id);
        
        if (task == null)
        {
            _logger.LogWarning($"Task with id {id} not found.");
            return OperationResult.Fail($"Task with id {id} not found.");
        }

        task.IsDone = true;
        await _repository.UpdateTaskAsync(task);
        _logger.LogInformation($"Task {id} marked as done.");
        return OperationResult.Ok($"Task {id} is done.");
    }

    public async Task<OperationResult> UpdateTaskTitleAsync(int id, string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning("Attempt to rename task {TaskId} with empty title.", id);
            return OperationResult.Fail("Task title should not be empty.");
        }

        TaskItem? task = await _repository.GetTaskByIdAsync(id);
        
        if (task is null)
            return OperationResult.Fail($"Task with id {id} not found.");
        
        task.Title = title;
        await _repository.UpdateTaskAsync(task);
        
        _logger.LogInformation($"Task {id} renamed to {title}.");
        return OperationResult.Ok($"Task updated: {title}.");
    }

    public async Task<OperationResult> DeleteTaskAsync(int id)
    {
        TaskItem task = await _repository.GetTaskByIdAsync(id);
        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found for delete.", id);
            return OperationResult.Fail($"Task with id {id} not found.");
        }

        await _repository.DeleteTaskAsync(id);
        
        _logger.LogInformation($"Task {id} deleted.");
        return OperationResult.Ok("Task deleted");
    }

    public async Task<List<TaskItem>> GetTasksByFilterAsync(TaskFilter filter)
    {
        tasks = await _repository.GetAllTasksAsync();
        return filter switch
        {
            TaskFilter.All => tasks,
            TaskFilter.Completed => tasks.Where(t => t.IsDone).ToList(),
            TaskFilter.NotCompleted => tasks.Where(t => !t.IsDone).ToList(),
            _ => new List<TaskItem>()
        };
    }


    public async Task<List<TaskItem>> GetSortedTasks(TaskSortOption sortOption)
    {
        tasks = await _repository.GetAllTasksAsync();
        return sortOption switch
        {
            TaskSortOption.ById => tasks.OrderBy(t => t.Id).ToList(),
            TaskSortOption.ByTitle => tasks.OrderBy(t => t.Title).ToList(),
            TaskSortOption.ByCreatedAt => tasks.OrderBy(t => t.CreatedAt).ToList(),
            _ => new List<TaskItem>()
        };
    }

    public async Task<List<TaskItem>> SearchTasksAsync(string keyword)
    {
        tasks = await _repository.GetAllTasksAsync();
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<TaskItem>();

        return tasks
            .Where(t => t.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}