using TodoTg.Domain.Entities;
using TodoTg.Domain.ValueObj;
using Utilities.Application;

namespace TodoTg.Application.Models.Todos
{
    public class TodoPatch : IPatch<Todo>
    {
        public string? Title { get; set; }

        public bool? IsCompleted { get; set; }

        public string? Description { get; set; }

        public Priority? Priority { get; set; }

        public DateTime? Deadline { get; set; }

        public void Apply(Todo target)
        {
            if (Title != null)
                target.Title = Title;

            if (IsCompleted.HasValue)
                target.IsCompleted = IsCompleted.Value;

            if (Description != null)
                target.Description = Description;

            if (Priority.HasValue)
                target.Priority = Priority.Value;

            if (Deadline.HasValue)
                target.Deadline = Deadline.Value;
        }
    }
}
