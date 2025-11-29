using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class EditModel : PageModel
{
    private readonly ITaskService _taskService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITaskService taskService, ILogger<EditModel> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [BindProperty]
    public TaskItem TaskItem { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = await _taskService.GetByIdAsync(id.Value);
        if (task == null)
        {
            return NotFound();
        }

        TaskItem = task;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
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
            var existingTask = await _taskService.GetByIdAsync(TaskItem.Id);
            if (existingTask == null)
            {
                return NotFound();
            }

            await _taskService.UpdateAsync(TaskItem);
            TempData["SuccessMessage"] = "Задача успешно обновлена";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении задачи id={Id}: {Message}", TaskItem.Id, ex.Message);
            ModelState.AddModelError("", $"Ошибка при обновлении задачи: {ex.Message}");
            return Page();
        }
    }
}
