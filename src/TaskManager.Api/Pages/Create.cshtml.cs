using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TaskManager.Api.Models;
using TaskManager.Api.Services;

namespace TaskManager.Api.Pages;

public class CreateModel : PageModel
{
    private readonly ITaskService _service;

    public CreateModel(ITaskService service)
    {
        _service = service;
    }

    [BindProperty]
    public TaskItem TaskItem { get; set; } = new TaskItem();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // серверная валидация — вернём страницу с ошибками
            return Page();
        }

        await _service.CreateAsync(TaskItem);
        return RedirectToPage("Index");
    }
}