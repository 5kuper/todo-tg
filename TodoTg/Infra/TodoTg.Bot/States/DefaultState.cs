using Telegram.Bot;
using Telegram.Bot.Types;
using TodoTg.Application.Services.Abstractions;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class DefaultState(IUserService userService) : BaseAppState(userService), IDefaultState<TgBotChatData>
    {
        public override async Task HandleUpdateAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            await base.HandleUpdateAsync(ctx, bot, update);
            if (ctx.IsUpdateHandled) return;
        }
    }
}
