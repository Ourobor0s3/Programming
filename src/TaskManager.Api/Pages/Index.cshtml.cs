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
    public SearchResultDto? SearchResult { get; set; }
    public string? SearchQuery { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IsSearchMode => !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task OnGetAsync(string? q = null, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            SearchQuery = q;
            CurrentPage = pageNumber > 0 ? pageNumber : 1;
            PageSize = pageSize > 0 ? pageSize : 10;

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Режим поиска с пагинацией
                SearchResult = await _taskService.SearchAsync(SearchQuery, CurrentPage, PageSize);
                Tasks = SearchResult.Items;
            }
            else
            {
                // Обычный режим - все задачи
                Tasks = await _taskService.GetAllAsync();
            }
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