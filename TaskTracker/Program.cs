using TaskTracker.Models;
using TaskTracker.Repositories;
using TaskTracker.Services;

ITaskRepository repository = new SqliteTaskRepository();
TaskService taskService = new TaskService(repository);

while (true)
{
    await PauseAndShowMessageAsync();
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

    switch (choice)
    {
        case "1":
            ShowAllTasks(taskService.GetAllTasks());
            break;
        
        case "2":
            Console.Write("Enter task name: ");
            string? title = Console.ReadLine();
            string addResult = taskService.AddTask(title ?? "");
            Console.WriteLine(addResult);
            break;
        
        case "3":
            Console.Write("Enter task ID: ");
            if (int.TryParse(Console.ReadLine(), out int doneId))
            {
                string doneResult = taskService.MarkTaskAsDone(doneId);
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
                string deleteResult = taskService.DeleteTask(deleteId);
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
            List<TaskItem> foundTasks = taskService.SearchTasks(keyword ?? string.Empty);
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
                "1" => taskService.GetTasksByFilter(TaskFilter.All),
                "2" => taskService.GetTasksByFilter(TaskFilter.Completed),
                "3" => taskService.GetTasksByFilter(TaskFilter.NotCompleted),
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
                "1" => taskService.GetSortedTasks(TaskSortOption.ById),
                "2" => taskService.GetSortedTasks(TaskSortOption.ByTitle),
                "3" => taskService.GetSortedTasks(TaskSortOption.ByCreatedAt),
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
            int.TryParse(Console.ReadLine(), out int taskId);
            Console.Write("Enter new title: ");
            string taskTitle = Console.ReadLine();
            string updateTitleResult = taskService.UpdateTaskTitle(taskId, taskTitle);
            Console.WriteLine(updateTitleResult);
            break;
        
        case "0":
            Console.WriteLine("Exiting...");
            return;
        
        default:
            Console.WriteLine("Invalid choice.");
            break;
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
    
    static async Task PauseAndShowMessageAsync()
    {
        await Task.Delay(1000);
        Console.WriteLine("One second passed");
    }
}

