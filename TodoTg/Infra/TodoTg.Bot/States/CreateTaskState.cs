using Telegram.Bot;
using Telegram.Bot.Types;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class CreateTaskState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        public override async Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            await bot.SendMessage(ctx.Data.ChatId, Strings.EnterTaskTitle);
        }

        public override async Task OnMessageAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var msg = update.Message?.Text;

            if (string.IsNullOrWhiteSpace(msg) || msg.Length < 3)
            {
                await bot.SendMessage(ctx.Data.ChatId, Strings.TitleTooShort);
                return;
            }

            var todo = await todoService.CreateAsync(new() { Title = msg, UserId = ctx.Data.GetUserId() });

            await bot.SendMessage(ctx.Data.ChatId, string.Format(Strings.TaskCreated, todo.Title));
            ctx.ChangeStateToDefault();
        }
    }
}
