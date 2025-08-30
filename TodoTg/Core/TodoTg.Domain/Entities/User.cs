using TodoTg.Domain.ValueObj;
using Utilities.Common.Domain;

namespace TodoTg.Domain.Entities
{
    public class User : Entity
    {
        public required string Name { get; set; }

        public AppLanguage Language { get; set; }

        public required virtual IList<Todo> Todos { get; set; }

        public long? TgChatId { get; set; }
    }
}
