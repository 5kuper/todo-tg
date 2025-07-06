using Utilities.Common.Domain;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot
{
    public class TgBotChatData : Entity, IChatData
    {
        public long ChatId { get; set; }

        public Guid? UserId { get; set; }

        public Guid GetUserId() => UserId ?? throw new InvalidOperationException("UserId is null.");
    }
}
