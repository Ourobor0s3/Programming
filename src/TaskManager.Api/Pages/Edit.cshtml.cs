using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class EditModel : PageModel
{
    private readonly ITaskService _taskService;

    public EditModel(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [BindProperty]
    public TaskItem Task { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        Task = task;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        // Убеждаемся, что ID совпадает
        Task.Id = id;

        // Проверяем существование задачи
        var existing = await _taskService.GetByIdAsync(id);
        if (existing == null)
        {
            TempData["ErrorMessage"] = "Задача не найдена";
            return RedirectToPage("/Index");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var updated = await _taskService.UpdateAsync(Task);
            /*if (!updated)
            {
                ModelState.AddModelError(string.Empty, "Не удалось обновить задачу");
                return Page();
            }*/

            TempData["SuccessMessage"] = $"Задача \"{Task.Title}\" успешно обновлена";
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Ошибка при обновлении задачи: {ex.Message}");
            return Page();
        }
    }
}