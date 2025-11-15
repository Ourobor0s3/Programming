using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;
        private readonly object _lock = new();

        public TaskService()
        {
            // Seed example tasks
            AddTask(new TaskItem { Title = "Пример задачи 1", Description = "Описание 1" });
            AddTask(new TaskItem { Title = "Пример задачи 2", Description = "Описание 2" });
        }

        public List<TaskItem> GetAll() => _tasks.ToList();

        public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

        public TaskItem AddTask(TaskItem task)
        {
            lock (_lock)
            {
                task.Id = _nextId++;
                _tasks.Add(task);
                return task;
            }
        }

        public bool UpdateTask(TaskItem task)
        {
            lock (_lock)
            {
                var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
                if (existing == null) return false;
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.IsDone = task.IsDone;
                existing.DueDate = task.DueDate;
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_lock)
            {
                var item = _tasks.FirstOrDefault(t => t.Id == id);
                if (item == null) return false;
                _tasks.Remove(item);
                return true;
            }
        }

        // Async implementations for interface
        public Task<List<TaskItem>> GetAllAsync() => Task.FromResult(GetAll());

        public Task<TaskItem?> GetByIdAsync(int id) => Task.FromResult(GetById(id));

        public Task<TaskItem> CreateAsync(TaskItem task) => Task.FromResult(AddTask(task));

        public Task<bool> UpdateAsync(TaskItem task) => Task.FromResult(UpdateTask(task));

        public Task<bool> DeleteAsync(int id) => Task.FromResult(Delete(id));
    }
}