using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _taskService.CreateAsync(TaskItem);
            TempData["SuccessMessage"] = "Задача успешно создана";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании задачи");
            ModelState.AddModelError("", "Ошибка при создании задачи. Попробуйте еще раз.");
            return Page();
        }
    }
}
