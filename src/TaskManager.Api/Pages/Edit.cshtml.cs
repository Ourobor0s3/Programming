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
        if (!ModelState.IsValid)
        {
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
            _logger.LogError(ex, "Ошибка при обновлении задачи id={Id}", TaskItem.Id);
            ModelState.AddModelError("", "Ошибка при обновлении задачи. Попробуйте еще раз.");
            return Page();
        }
    }
}
