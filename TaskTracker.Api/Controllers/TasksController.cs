using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Models;
using TaskTracker.Api.Services;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskItem>>> GetAllTasks()
    {
        List<TaskItem> tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem?>> GetTaskByIdAsync(int id)
    {
        TaskItem? task = await _taskService.GetTaskByIdAsync(id);

        if (task is null)
            return NotFound();
        
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskRequest request)
    {
        string result = await _taskService.AddTaskAsync(request.Title);

        if (result == "Task title should not be empty.")
            return BadRequest(new { message = result });

        return Ok(new { message = result });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItem>> UpdateTaskTitle(int id, [FromBody] UpdateTaskTitleRequest request)
    {
        string result = await _taskService.UpdateTaskTitleAsync(id, request.Title);
        if (result == "Task title should not be empty.")
            return BadRequest(new { message = result });

        if (result.Contains("not found"))
            return NotFound(new { message = result });
        
        return Ok(new { message = result });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<TaskItem?>> DeleteTaskByIdAsync(int id)
    {
        string result = await _taskService.DeleteTaskAsync(id);
        if (result.Contains("not found"))
            return NotFound(new {message = result });
        
        return Ok(new { message = result });
    }
}