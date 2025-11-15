using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages
{
    public class EditModel : PageModel
    {
        private readonly TaskService _taskService;

        public EditModel(TaskService taskService)
        {
            _taskService = taskService;
        }

        [BindProperty]
        public TaskItem Task { get; set; } = null!;

        public IActionResult OnGet(int id)
        {
            var task = _taskService.GetById(id);
            if (task == null) return NotFound();
            Task = task;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            // Убеждаемся, что ID из маршрута совпадает с ID в модели
            if (Task.Id != id)
            {
                Task.Id = id;
            }

            if (!ModelState.IsValid)
            {
                // Перезагружаем задачу для отображения формы
                var existing = _taskService.GetById(id);
                if (existing != null)
                {
                    Task = existing;
                }
                return Page();
            }

            try
            {
                var ok = _taskService.UpdateTask(Task);
                if (!ok)
                {
                    ModelState.AddModelError("", "Задача не найдена");
                    var existing = _taskService.GetById(id);
                    if (existing != null)
                    {
                        Task = existing;
                    }
                    return Page();
                }
                TempData["SuccessMessage"] = "Задача успешно обновлена";
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении задачи: {ex.Message}");
                var existing = _taskService.GetById(id);
                if (existing != null)
                {
                    Task = existing;
                }
                return Page();
            }
        }
    }
}