using Telegram.Bot;
using Telegram.Bot.Types;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.States;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot
{
    public class BaseAppState(IUserService userService) : IBotState<TgBotChatData>
    {
        protected IUserService UserService { get; } = userService;

        public virtual async Task HandleUpdateAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            await update.CallbackQuery.AnswerIfEmpty(bot);
            ctx.Data.UserId ??= await UserService.EnsureCreatedForTelegram(new() { Name = ctx.Name, TgChatId = ctx.Data.ChatId });

            ctx.IsUpdateHandled = false;
            switch (update.Message?.Text)
            {
                case "/start":
                {
                    await bot.SendMessage(ctx.Data.ChatId, "Hello!");
                    ctx.IsUpdateHandled = true;
                    break;
                }
                case "/newtask":
                {
                    await bot.SendMessage(ctx.Data.ChatId, "Enter a task title:");
                    ctx.ChangeState<CreateTaskState>();
                    ctx.IsUpdateHandled = true;
                    break;
                }
                case "/tasks":
                {
                    ctx.ChangeState<TaskListState>();
                    update.Message.Text = null;
                    await ctx.HandleUpdateAsync(bot, update);
                    ctx.IsUpdateHandled = true;
                    break;
                }
            }
        }
    }
}
