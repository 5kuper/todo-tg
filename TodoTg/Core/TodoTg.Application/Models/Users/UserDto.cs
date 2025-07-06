namespace TodoTg.Application.Models.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public long? TgChatId { get; set; }
    }
}
