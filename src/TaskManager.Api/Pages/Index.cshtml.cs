using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class IndexModel : PageModel
{
    private readonly ITaskService _taskService;

    public IndexModel(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public List<TaskItem> Tasks { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Tasks = await _taskService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                TempData["ErrorMessage"] = "Задача не найдена";
                return RedirectToPage();
            }

            var result = await _taskService.DeleteAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = $"Задача \"{task.Title}\" успешно удалена";
            }
            else
            {
                TempData["ErrorMessage"] = "Не удалось удалить задачу";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка при удалении задачи: {ex.Message}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostImportAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["ErrorMessage"] = "Файл не выбран или пуст";
            return RedirectToPage();
        }

        if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Поддерживаются только JSON файлы";
            return RedirectToPage();
        }

        try
        {
            using var stream = new StreamReader(file.OpenReadStream());
            var jsonContent = await stream.ReadToEndAsync();
            var tasks = System.Text.Json.JsonSerializer.Deserialize<List<TaskItem>>(jsonContent, 
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (tasks == null || tasks.Count == 0)
            {
                TempData["ErrorMessage"] = "Файл не содержит задач или имеет неверный формат";
                return RedirectToPage();
            }

            int imported = 0;
            int errors = 0;

            foreach (var task in tasks)
            {
                try
                {
                    task.Id = 0; // Убеждаемся, что ID не установлен
                    await _taskService.CreateAsync(task);
                    imported++;
                }
                catch
                {
                    errors++;
                }
            }

            TempData["SuccessMessage"] = $"Импортировано задач: {imported}. Ошибок: {errors}";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка при импорте: {ex.Message}";
        }

        return RedirectToPage();
    }
}
