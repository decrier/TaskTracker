using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskTracker.Tests;

namespace TasskTracker.Tests;

public class TaskServiceTests
{
    private static async Task<TaskService> CreateServiceAsync()
    {
        FakeTaskRepository repository = new FakeTaskRepository();
        TaskService service = new TaskService(repository, NullLogger<TaskService>.Instance);
        await service.InitializeAsync();
        return service;
    }

    [Fact]
    public async Task AddTaskAsync_Should_Add_New_Task()
    {
        TaskService service = await CreateServiceAsync();

        string result = await service.AddTaskAsync("Learn C#");

        List<TaskItem> tasks = await service.GetAllTasksAsync();
        
        Assert.Equal("Task added: Learn C#.", result);
        Assert.Single(tasks);
        Assert.Equal("Learn C#", tasks[0].Title);
        Assert.False(tasks[0].IsDone);
    }

    [Fact]
    public async Task AddTaskAsync_Should_Return_Error_When_Title_Is_Empty()
    {
        TaskService service = await CreateServiceAsync();
        string result = await service.AddTaskAsync("");
        
        List<TaskItem> tasks = await service.GetAllTasksAsync();
        Assert.Equal("Task title should not be empty.", result);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task DeleteTaskAsync_Should_Remove_Task()
    {
        TaskService service = await CreateServiceAsync();
        await service.AddTaskAsync("Task to be deleted");

        string result = await service.DeleteTaskAsync(1);
        List<TaskItem> tasks = await service.GetAllTasksAsync();
        
        Assert.Equal("Task deleted", result);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task UpdateTaskTitleAsync_Should_Change_Task_Title()
    {
        TaskService service = await CreateServiceAsync();

        await service.AddTaskAsync("Old task");
        string result = await service.UpdateTaskTitleAsync(1, "New title");
        List<TaskItem> tasks = await service.GetAllTasksAsync();
        
        Assert.Equal("Task updated: New title.", result);
        Assert.Equal("New title", tasks[0].Title);
    }

    [Fact]
    public async Task SearchTaskAsync_Should_Return_Matching_Tasks()
    {
        TaskService service = await CreateServiceAsync();
        await service.AddTaskAsync("Learn C#");
        await service.AddTaskAsync("New task");

        List<TaskItem> result = await service.SearchTasksAsync("learn");
        
        Assert.Single(result);
        Assert.Equal("Learn C#", result[0].Title);
    }

    [Fact]
    public async Task GetTasksByFilterAsync_Should_Return_Only_Completed_Tasks()
    {
        TaskService service = await CreateServiceAsync();
        await service.AddTaskAsync("Task 1");
        await service.AddTaskAsync("Task 2");

        await service.MarkTaskAsDoneAsync(2);

        List<TaskItem> tasks = await service.GetTasksByFilterAsync(TaskFilter.Completed);
        Assert.Single(tasks);
        Assert.Equal(2, tasks[0].Id);
        Assert.True(tasks[0].IsDone);
    }
}