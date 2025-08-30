using TodoTg.Domain.ValueObj;

namespace TodoTg.Application.Models.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public AppLanguage Language { get; set; }

        public long? TgChatId { get; set; }
    }
}
