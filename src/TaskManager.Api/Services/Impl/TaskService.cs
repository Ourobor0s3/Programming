using TaskManager.Api.Models;

namespace TaskManager.Api.Services.Impl
{
    public class TaskService : ITaskService
    {
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public Task<List<TaskItem>> GetAllAsync() => Task.FromResult(_tasks.ToList());

        public Task<TaskItem?> GetByIdAsync(int id) =>
            Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

        public Task<TaskItem> CreateAsync(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return Task.FromResult(task);
        }

        public Task<bool> UpdateAsync(TaskItem updated)
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == updated.Id);
            if (existing == null) return Task.FromResult(false);
            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.IsDone = updated.IsDone;
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(int id)
        {
            return Task.FromResult(_tasks.RemoveAll(t => t.Id == id) > 0);
        }
    }
}