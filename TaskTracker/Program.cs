using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskTracker.Configurations;
using TaskTracker.Models;
using TaskTracker.Repositories;
using TaskTracker.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddConfiguration(configuration.GetSection("Logging"));
    builder.AddConsole();
});

services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));

services.AddSingleton<ITaskRepository, SqliteTaskRepository>();
services.AddSingleton<TaskService>();

using ServiceProvider serviceProvider = services.BuildServiceProvider();

TaskService taskService = serviceProvider.GetRequiredService<TaskService>();
ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();
SqliteTaskRepository sqliteRepository = (SqliteTaskRepository)serviceProvider.GetRequiredService<ITaskRepository>();

await sqliteRepository.InitializeDatabaseAsync();
await taskService.InitializeAsync();

while (true)
{
    Console.WriteLine("=== Task Tracker ===");
    Console.WriteLine("1. Show all tasks");
    Console.WriteLine("2. Add new task");
    Console.WriteLine("3. Mark task as done");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Search for tasks");
    Console.WriteLine("6. Filter tasks");
    Console.WriteLine("7. Sort tasks");
    Console.WriteLine("8. Edit task title");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
    
    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                ShowAllTasks(await taskService.GetAllTasksAsync());
                break;
        
            case "2":
                Console.Write("Enter task name: ");
                string? title = Console.ReadLine();
                string addResult = await taskService.AddTaskAsync(title ?? "");
                Console.WriteLine(addResult);
                break;
        
            case "3":
                Console.Write("Enter task ID: ");
                if (int.TryParse(Console.ReadLine(), out int doneId))
                {
                    string doneResult = await taskService.MarkTaskAsDoneAsync(doneId);
                    Console.WriteLine(doneResult);
                }
                else
                {
                    Console.WriteLine("Invalid task ID.");
                }
                break;
        
            case "4":
                Console.Write("Enter task ID: ");
                if (int.TryParse(Console.ReadLine(), out int deleteId))
                {
                    string deleteResult = await taskService.DeleteTaskAsync(deleteId);
                    Console.WriteLine(deleteResult);
                }
                else
                {
                    Console.WriteLine("Invalid task ID.");
                }
                break;
        
            case "5":
                Console.Write("Enter keyword: ");
                string? keyword = Console.ReadLine();
                List<TaskItem> foundTasks = await taskService.SearchTasksAsync(keyword ?? string.Empty);
                ShowAllTasks(foundTasks);
                break;
        
            case "6":
                Console.WriteLine("1. All tasks");
                Console.WriteLine("2. Completed tasks");
                Console.WriteLine("3. Incompleted tasks");
                Console.Write("Enter your choice: ");
            
                string? filterChoice = Console.ReadLine();
                List<TaskItem> filteredTasks = filterChoice switch
                {
                    "1" => await taskService.GetTasksByFilterAsync(TaskFilter.All),
                    "2" => await taskService.GetTasksByFilterAsync(TaskFilter.Completed),
                    "3" => await taskService.GetTasksByFilterAsync(TaskFilter.NotCompleted),
                    _ => new List<TaskItem>()
                };

                if (filterChoice is not ("1" or "2" or "3"))
                {
                    Console.WriteLine("Invalid choice.");
                }
                else
                {
                    ShowAllTasks(filteredTasks);
                }
                break;
        
            case "7":
                Console.WriteLine("1. Sort by ID");
                Console.WriteLine("2. Sort by title");
                Console.WriteLine("3. Sort by creation date");
                Console.Write("Enter your choice: ");
            
                string ? sortChoice = Console.ReadLine();
                List<TaskItem> sortedTasks = sortChoice switch
                {
                    "1" => await taskService.GetSortedTasks(TaskSortOption.ById),
                    "2" => await taskService.GetSortedTasks(TaskSortOption.ByTitle),
                    "3" => await taskService.GetSortedTasks(TaskSortOption.ByCreatedAt),
                    _ => new List<TaskItem>()
                };

                if (sortChoice is not ("1" or "2" or "3"))
                {
                    Console.WriteLine("Invalid choice.");
                }
                else
                {
                    ShowAllTasks(sortedTasks);
                }
                break;
        
            case "8":
                Console.Write("Enter task ID: ");
                if (int.TryParse(Console.ReadLine(), out int taskId))
                {
                    Console.Write("Enter new title: ");
                    string? taskTitle = Console.ReadLine();
                    string updateTitleResult = await taskService.UpdateTaskTitleAsync(taskId, taskTitle ?? "");
                    Console.WriteLine(updateTitleResult);
                }
                else
                {
                    Console.WriteLine("Invalid task ID.");
                }
                break;
        
            case "0":
                logger.LogInformation("Application is shutting down.");
                Console.WriteLine("Exiting...");
                return;
        
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    catch (Exception e)
    {
        logger.LogError(e, "Unhandled application error");
        Console.WriteLine("Application error: " +e.Message);
    }
    

    static void ShowAllTasks(List<TaskItem> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }
        
        foreach (TaskItem task in tasks)
        {
            string status = task.IsDone ? "[X]" : "[ ]";
            Console.WriteLine($"{task.Id}. {status} {task.Title} | Created: {task.CreatedAt:dd.MM.yyyy HH:mm}");
        }
    }
}

