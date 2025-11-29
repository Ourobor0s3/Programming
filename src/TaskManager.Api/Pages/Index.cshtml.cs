using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class IndexModel : PageModel
{
    private readonly ITaskService _taskService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITaskService taskService, ILogger<IndexModel> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    public List<TaskItem> Tasks { get; set; } = new();

    public async Task OnGetAsync()
    {
        try
        {
            Tasks = await _taskService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке задач");
            ModelState.AddModelError("", "Ошибка при загрузке задач. Попробуйте обновить страницу.");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var result = await _taskService.DeleteAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Задача успешно удалена";
            }
            else
            {
                TempData["ErrorMessage"] = "Задача не найдена";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении задачи id={Id}", id);
            TempData["ErrorMessage"] = "Ошибка при удалении задачи";
        }

        return RedirectToPage();
    }
}