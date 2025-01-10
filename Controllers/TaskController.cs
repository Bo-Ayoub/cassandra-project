using cassandra_app.DTO;
using cassandra_app.Services;
using Microsoft.AspNetCore.Mvc;
using Task = cassandra_app.Models.Task;

namespace cassandra_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly CassandraService _cassandraService;

        public TaskController(CassandraService cassandraService)
        {
            _cassandraService = cassandraService;
        }

        [HttpGet]
        public async Task<IEnumerable<Models.Task>> GetAllTasks()
        {
            var rows = await _cassandraService.GetAllTasks();
            var tasks = new List<Task>();

            foreach (var row in rows)
            {
                tasks.Add(new Task
                {
                    Id = row.GetValue<Guid>("id"),
                    Title = row.GetValue<string>("title"),
                    Description = row.GetValue<string>("description"),
                    Completed = row.GetValue<bool>("completed")
                });
            }

            return tasks;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> GetTaskById(Guid id)
        {
            var row = await _cassandraService.GetTaskById(id);
            if (row == null)
                return NotFound();

            var task = new Task
            {
                Id = row.GetValue<Guid>("id"),
                Title = row.GetValue<string>("title"),
                Description = row.GetValue<string>("description"),
                Completed = row.GetValue<bool>("completed")
            };

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] Task task)
        {
            task.Id = Guid.NewGuid();  // Automatically assign a new ID
            await _cassandraService.InsertTask(task.Id, task.Title, task.Description, task.Completed);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _cassandraService.DeleteTask(id);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            if (taskUpdateDto == null)
            {
                return BadRequest("Task data is required.");
            }

            await _cassandraService.UpdateTask(id, taskUpdateDto.Title, taskUpdateDto.Description, taskUpdateDto.Completed);
            return NoContent(); // HTTP 204 No Content
        }

    }
}
