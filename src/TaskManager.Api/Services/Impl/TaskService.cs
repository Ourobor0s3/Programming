using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _dbContext;

        public TaskService(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _dbContext.Tasks.AsNoTracking().OrderBy(t => t.Id).ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _dbContext.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            task.Id = 0;
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            var existing = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
            if (existing == null) throw new InvalidOperationException($"Task with id {task.Id} not found.");
            // Пример — копируем поля; измените по модели
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.IsDone = task.IsDone;
            existing.DueDate = task.DueDate;
            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null) return false;
            _dbContext.Tasks.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<(List<TaskItem> Items, int TotalCount)> SearchRawAsync(string q, int page, int pageSize)
        {
            var query = _dbContext.Tasks.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim().ToLower();
                query = query.Where(t => (t.Title != null && t.Title.ToLower().Contains(q)) ||
                                         (t.Description != null && t.Description.ToLower().Contains(q)));
            }

            var total = await query.CountAsync();
            var items = await query.OrderBy(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public async Task<SearchResultDto> SearchAsync(string q, int page, int pageSize)
        {
            var (items, total) = await SearchRawAsync(q, page, pageSize);
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            return new SearchResultDto
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task ExportToFileAsync(string path)
        {
            var items = await GetAllAsync();
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            await File.WriteAllTextAsync(path, json);
        }
    }
}