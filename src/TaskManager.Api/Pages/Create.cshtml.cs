using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class CreateModel : PageModel
{
    private readonly ITaskService _taskService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(ITaskService taskService, ILogger<CreateModel> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [BindProperty]
    public TaskItem TaskItem { get; set; } = new();

    public void OnGet()
    {
        TaskItem.DueDate = DateTime.Now.AddDays(1);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Убеждаемся, что Id = 0 для новой задачи
        TaskItem.Id = 0;
        
        // Нормализуем Description (если пустая строка, делаем null)
        if (string.IsNullOrWhiteSpace(TaskItem.Description))
        {
            TaskItem.Description = null;
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Валидация не прошла. Ошибки: {Errors}", 
                string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return Page();
        }

        try
        {
            await _taskService.CreateAsync(TaskItem);
            TempData["SuccessMessage"] = "Задача успешно создана";
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Ошибка базы данных при создании задачи: {Message}", dbEx.Message);
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            ModelState.AddModelError("", $"Ошибка при сохранении в базу данных: {innerMessage}");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании задачи: {Message}", ex.Message);
            ModelState.AddModelError("", $"Ошибка при создании задачи: {ex.Message}");
            return Page();
        }
    }
}
