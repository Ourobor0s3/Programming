using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem> UpdateAsync(TaskItem task);
        Task<bool> DeleteAsync(int id);

        // Для export/search
        Task<(List<TaskItem> Items, int TotalCount)> SearchRawAsync(string q, int page, int pageSize);
        Task<SearchResultDto> SearchAsync(string q, int page, int pageSize);
        Task ExportToFileAsync(string path);
    }
}