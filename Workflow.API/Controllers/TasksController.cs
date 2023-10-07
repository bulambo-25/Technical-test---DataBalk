using Microsoft.AspNetCore.Mvc;
using Workflow.API.Data;
using Workflow.API.Models;
using Workflow.API.Models.Dto;

namespace Workflow.API.Controllers
{
    [Route("api/Task")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _logger;
        private readonly ApplicationDbContext _db;

        public TasksController(ILogger<TasksController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        // Get All Tasks
        [HttpGet]
        [Route("TaskList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<TasksDto>> GetTasks()
        {
            _logger.LogInformation("Getting all tasks");
            return Ok(_db.tTask.ToList());
        }

        //Get Single Task
       [HttpGet("{id:int}")]
       [ProducesResponseType(StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TasksDto>> GetTask(int id)
        {

            if (id == 0)
                return BadRequest();

            var task = _db.tTask.FirstOrDefault(u => u.Id == id);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        // Create New Task
        [HttpPost]
        [Route("CreateTask")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TasksDto> CreateTask([FromBody] TasksDto tasksDto)
        {

            if (tasksDto == null)
                return BadRequest(tasksDto);
            if (tasksDto.Id > 0)
                return StatusCode(StatusCodes.Status409Conflict);

            // check if user exist already

            if (_db.tTask.FirstOrDefault(u => u.Title.ToLower() == tasksDto.Title.ToLower()) != null)
            {
                _logger.LogError("Error Occur: Task Already Exist");
                return BadRequest(ModelState);

            }

            Tasks taskObj = new()
            {
                Title = tasksDto.Title,
                Description = tasksDto.Description,
                UserId = tasksDto.UserId.Id,
                DueDate = tasksDto.DueDate
            };

            _db.tTask.Add(taskObj);
            _db.SaveChanges();

            return Ok("Task Successfully Created");
        }

        [HttpPut("EditTask/{id:int}", Name = "EditTask")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult EditTask(int id, [FromBody] TasksDto tasksDto)
        {
            try
            {
                if (tasksDto == null || id != tasksDto.Id)
                    return BadRequest();

                var existingTask = _db.tTask.FirstOrDefault(u => u.Id == id);

                if (existingTask == null)
                    return NotFound();

                // Update existing task properties
                existingTask.Description = tasksDto.Description;
                existingTask.UserId = tasksDto.UserId.Id;
                existingTask.DueDate = tasksDto.DueDate;

                _db.tTask.Update(existingTask);
                _db.SaveChanges();

                return Ok("Task Updated Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error", ex.Message);
                return BadRequest();
            }
        }


        // Delete Existing Task
        [HttpDelete("DeleteTask/{id:int}", Name = "DeleteTask")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteTask(int id)
        {

            if (id == 0)
                return BadRequest();

            var task = _db.tTask.FirstOrDefault(u => u.Id == id);

            if (task == null)
                return NotFound();

            _db.tTask.Remove(task);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
