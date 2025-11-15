using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TaskManager.Api.Pages
{
    public class IndexModel : PageModel
    {
        private readonly TaskService _taskService;

        public IndexModel(TaskService taskService)
        {
            _taskService = taskService;
        }

        public List<TaskItem> Tasks { get; set; } = new();

        public void OnGet()
        {
            Tasks = _taskService.GetAll();
        }

        public IActionResult OnPostDelete(int id)
        {
            var result = _taskService.Delete(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Задача не найдена";
            }
            else
            {
                TempData["SuccessMessage"] = "Задача успешно удалена";
            }
            return RedirectToPage();
        }
    }
}