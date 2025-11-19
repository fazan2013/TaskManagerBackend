using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;


namespace TaskManager.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetAllAsync(string? search, string? sortBy, bool? isCompleted);
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(TaskItem task);
    }
}
