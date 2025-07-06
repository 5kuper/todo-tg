using TodoTg.Domain.Entities;
using TodoTg.Domain.ValueObj;

namespace TodoTg.Application.Models.Todos
{
    public class TodoDto
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }

        public bool IsCompleted { get; set; }

        public virtual User User { get; set; } = null!;

        public Guid UserId { get; set; }

        public string? Description { get; set; }

        public Priority? Priority { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
