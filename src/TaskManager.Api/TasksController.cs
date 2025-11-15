using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
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

        public TasksController(ITaskService service, IWebHostEnvironment environment)
        {
            _service = service;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskItem>>> GetAll()
        {
            try
            {
                var tasks = await _service.GetAllAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при получении списка задач", message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> Get(int id)
        {
            try
            {
                var task = await _service.GetByIdAsync(id);
                if (task == null) 
                    return NotFound(new { error = "Задача не найдена", id });
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при получении задачи", message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Ошибка валидации", errors = ModelState });
            }

            try
            {
                var created = await _service.CreateAsync(task);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при создании задачи", message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Ошибка валидации", errors = ModelState });
            }

            if (id != task.Id)
            {
                return BadRequest(new { error = "Идентификатор в URL не совпадает с идентификатором в теле запроса" });
            }

            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { error = "Задача не найдена", id });

                var ok = await _service.UpdateAsync(task);
                if (!ok) 
                    return NotFound(new { error = "Задача не найдена", id });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при обновлении задачи", message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _service.DeleteAsync(id);
                if (!ok) 
                    return NotFound(new { error = "Задача не найдена", id });
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при удалении задачи", message = ex.Message });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            try
            {
                var tasks = await _service.GetAllAsync();
                
                // Сохранить в файл
                var filePath = Path.Combine(_environment.ContentRootPath, "tasks_export.json");
                var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                await System.IO.File.WriteAllTextAsync(filePath, json);
                
                // Вернуть JSON в ответе
                return Ok(new { 
                    message = "Экспорт выполнен успешно", 
                    filePath = filePath,
                    tasks = tasks,
                    count = tasks.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ошибка при экспорте задач", message = ex.Message });
            }
        }
    }
}