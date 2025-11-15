using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages
{
    public class CreateModel : PageModel
    {
        private readonly TaskService _taskService;

        public CreateModel(TaskService taskService)
        {
            _taskService = taskService;
        }

        [BindProperty]
        public TaskItem Task { get; set; } = new TaskItem();

        public void OnGet()
        {
            // Инициализируем задачу с дефолтными значениями
            Task = new TaskItem
            {
                DueDate = DateTime.Now
            };
        }

        public IActionResult OnPost()
        {
            // Проверяем валидацию модели
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Убеждаемся, что ID не установлен (будет установлен сервисом)
                Task.Id = 0;
                
                var created = _taskService.AddTask(Task);
                if (created != null)
                {
                    TempData["SuccessMessage"] = "Задача успешно создана";
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError("", "Не удалось создать задачу");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании задачи: {ex.Message}");
                return Page();
            }
        }
    }
}