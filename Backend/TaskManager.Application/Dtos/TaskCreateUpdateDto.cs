using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.Dtos
{
    public class TaskCreateUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        [Range(0, 2)]
        public int Priority { get; set; } = 0;
    }
}
