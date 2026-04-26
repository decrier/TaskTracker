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
    public async Task<ActionResult<TaskItem>> GetTaskByIdAsync(int id)
    {
        var result = await _taskService.GetTaskByIdAsync(id);

        if (!result.Success || result.Data == null)
            return NotFound(new { message = result.Message});
        
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var result = await _taskService.AddTaskAsync(request.Title);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItem>> UpdateTaskTitle(int id, [FromBody] UpdateTaskTitleRequest request)
    {
        var result = await _taskService.UpdateTaskTitleAsync(id, request.Title);
        if (!result.Success)
        {
            if (result.Message.Contains("not found"))
                return NotFound(new { message = result.Message });
            
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteTaskByIdAsync(int id)
    {
        var result = await _taskService.DeleteTaskAsync(id);
        
        if (!result.Success)
            return NotFound(new {message = result.Message });
        
        return Ok(new { message = result.Message });
    }
}