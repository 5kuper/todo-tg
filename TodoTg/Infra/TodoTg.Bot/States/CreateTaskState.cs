using Telegram.Bot;
using Telegram.Bot.Types;
using TodoTg.Application.Services.Abstractions;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class CreateTaskState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        public override async Task HandleUpdateAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            await base.HandleUpdateAsync(ctx, bot, update);
            if (ctx.IsUpdateHandled) return;

            var msg = update.Message?.Text;

            if (string.IsNullOrWhiteSpace(msg) || msg.Length < 3)
            {
                await bot.SendMessage(ctx.Data.ChatId, "The title is too short. Enter it again:");
                return;
            }

            var todo = await todoService.CreateAsync(new() { Title = msg, UserId = ctx.Data.GetUserId() });

            await bot.SendMessage(ctx.Data.ChatId, $"Task '{todo.Title}' has been created.");
            ctx.ChangeState<DefaultState>();
        }
    }
}
