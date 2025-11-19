using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Dtos;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(string? search, string? sortBy, bool? isCompleted);
        Task<TaskDto?> GetTaskAsync(int id);
        Task<TaskDto> CreateTaskAsync(TaskCreateUpdateDto dto);
        Task<TaskDto?> UpdateTaskAsync(int id, TaskCreateUpdateDto dto);
        Task<bool> DeleteTaskAsync(int id);
    }
}
