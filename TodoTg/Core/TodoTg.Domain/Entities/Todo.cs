using TodoTg.Domain.ValueObj;
using Utilities.Common.Domain;

namespace TodoTg.Domain.Entities
{
    public class Todo : Entity
    {
        public required string Title { get; set; }

        public bool IsCompleted { get; set; }

        public virtual User User { get; set; } = null!;

        public Guid UserId { get; set; }

        public string? Description { get; set; }

        public Priority? Priority { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
