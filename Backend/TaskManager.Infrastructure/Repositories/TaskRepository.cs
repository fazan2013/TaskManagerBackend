using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
//using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;

        public TaskRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TaskItem>> GetAllAsync(string? search, string? sortBy, bool? isCompleted)
        {
            var query = _db.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t =>
                    t.Title.Contains(search) ||
                    (t.Description != null && t.Description.Contains(search)));
            }

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "title" => query.OrderBy(t => t.Title),
                "duedate" => query.OrderBy(t => t.DueDate),
                "priority" => query.OrderByDescending(t => t.Priority),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(TaskItem task)
        {
            task.UpdatedAt = DateTime.UtcNow;
            _db.Tasks.Update(task);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem task)
        {
            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }
    }
}
