namespace TodoTg.Application.Models.Users
{
    public class UserInput
    {
        public required string Name { get; set; }

        public long? TgChatId { get; set; }
    }
}
