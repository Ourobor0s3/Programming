using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService service, IWebHostEnvironment environment, ILogger<TasksController> logger)
        {
            _service = service;
            _environment = environment;
            _logger = logger;
        }

        // GET api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var items = await _service.GetAllAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задач");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при получении задач", message = ex.Message });
            }
        }

        // GET api/tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { error = "NotFound", message = $"Задача с id={id} не найдена" });
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении задачи по id={Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при получении задачи", message = ex.Message });
            }
        }

        // POST api/tasks
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskItem model)
        {
            if (model == null) return BadRequest(new { error = "BadRequest", message = "Пустая модель" });

            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "ValidationError", errors = ModelState });
            }

            try
            {
                var created = await _service.CreateAsync(model);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании задачи");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при создании", message = ex.Message });
            }
        }

        // PUT api/tasks/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem model)
        {
            if (model == null) return BadRequest(new { error = "BadRequest", message = "Пустая модель" });

            if (id != model.Id)
                return BadRequest(new { error = "BadRequest", message = "Id в пути и в модели не совпадают" });

            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "ValidationError", errors = ModelState });
            }

            try
            {
                var exists = await _service.GetByIdAsync(id);
                if (exists == null) return NotFound(new { error = "NotFound", message = $"Задача с id={id} не найдена" });

                var updated = await _service.UpdateAsync(model);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении задачи id={Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при обновлении", message = ex.Message });
            }
        }

        // DELETE api/tasks/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exists = await _service.GetByIdAsync(id);
                if (exists == null) return NotFound(new { error = "NotFound", message = $"Задача с id={id} не найдена" });

                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении задачи id={Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при удалении", message = ex.Message });
            }
        }

        // GET api/tasks/export
        // возвращает JSON-массив всех задач; опционально записывает tasks_export.json на диск
        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] bool saveToDisk = false)
        {
            try
            {
                var items = await _service.GetAllAsync();
                var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });

                if (saveToDisk)
                {
                    var path = Path.Combine(_environment.ContentRootPath, "tasks_export.json");
                    await System.IO.File.WriteAllTextAsync(path, json);
                }

                return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", "tasks_export.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте задач");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при экспорте", message = ex.Message });
            }
        }

        // GET api/tasks/search?q=...&page=1&size=10
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q = "", [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            try
            {
                if (page <= 0) page = 1;
                if (size <= 0) size = 10;

                var result = await _service.SearchAsync(q ?? string.Empty, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске задач q={Q}", q);
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Ошибка при поиске", message = ex.Message });
            }
        }
    }
}