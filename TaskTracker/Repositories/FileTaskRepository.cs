// using System.Text.Json;
//
// namespace TaskTracker;
//
// public class FileTaskRepository : ITaskRepository
// {
//     private const string filePath = "tasks.json";
//     public List<TaskItem> GetAllTasks()
//     {
//         if(!File.Exists(filePath))
//             return new List<TaskItem>();
//         
//         string json = File.ReadAllText(filePath);
//         
//         List<TaskItem>? tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
//
//         return tasks ?? new List<TaskItem>();
//     }
//     
//     public void AddTask(List<TaskItem> tasks)
//     {
//         string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
//         {
//             WriteIndented = true
//         });
//         
//         File.WriteAllText(filePath, json);
//     }
// }