using TaskManager.Application.Dtos;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskDto>> GetTasksAsync(string? search, string? sortBy, bool? isCompleted)
        {
            var items = await _taskRepository.GetAllAsync(search, sortBy, isCompleted);
            return items.Select(MapToDto).ToList();
        }

        public async Task<TaskDto?> GetTaskAsync(int id)
        {
            var entity = await _taskRepository.GetByIdAsync(id);
            return entity is null ? null : MapToDto(entity);
        }

        public async Task<TaskDto> CreateTaskAsync(TaskCreateUpdateDto dto)
        {
            var entity = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description,
                DueDate = dto.DueDate,
                IsCompleted = dto.IsCompleted,
                Priority = dto.Priority,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _taskRepository.AddAsync(entity);
            return MapToDto(created);
        }

        public async Task<TaskDto?> UpdateTaskAsync(int id, TaskCreateUpdateDto dto)
        {
            var existing = await _taskRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return null;
            }

            existing.Title = dto.Title.Trim();
            existing.Description = dto.Description;
            existing.DueDate = dto.DueDate;
            existing.IsCompleted = dto.IsCompleted;
            existing.Priority = dto.Priority;
            existing.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(existing);
            return MapToDto(existing);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var existing = await _taskRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return false;
            }

            await _taskRepository.DeleteAsync(existing);
            return true;
        }

        private static TaskDto MapToDto(TaskItem entity)
        {
            return new TaskDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                DueDate = entity.DueDate,
                IsCompleted = entity.IsCompleted,
                Priority = entity.Priority,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
