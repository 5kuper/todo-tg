using TodoTg.Domain.ValueObj;
using Utilities.Common.Domain;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot
{
    public enum EditOption { Title, Description, Priority }

    public class TgBotChatData : Entity, IChatData
    {
        public long ChatId { get; set; }

        public int? FormMsgId { get; set; }


        public Guid? UserId { get; set; }

        public Guid GetUserId() => UserId ?? throw new InvalidOperationException("UserId is null.");


        public int CurrentTasksPage { get; set; }

        public Guid? SelectedTaskId { get; set; }

        public EditOption? SelectedEditOption { get; set; }
    }
}
