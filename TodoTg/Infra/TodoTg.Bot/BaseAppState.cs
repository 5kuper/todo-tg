using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.States;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot
{
    public class BaseAppState(IUserService userService) : IDefaultState<TgBotChatData>
    {
        protected IUserService UserService { get; } = userService;

        public virtual async Task HandleUpdateAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            await update.CallbackQuery.AnswerIfEmpty(bot);
            ctx.Data.UserId ??= await UserService.EnsureCreatedForTelegram(new() { Name = ctx.Name, TgChatId = ctx.Data.ChatId });

            switch (update.Type)
            {
                case UpdateType.Message:
                switch (update.Message?.Text)
                {
                    case "/start":
                    {
                        await ClearForm(ctx, bot);
                        await bot.SendMessage(ctx.Data.ChatId, "Hello!");
                        return;
                    }
                    case "/newtask":
                    {
                        await ClearForm(ctx, bot);
                        await bot.SendMessage(ctx.Data.ChatId, "Enter a task title:");
                        ctx.ChangeState<CreateTaskState>();
                        return;
                    }
                    case "/tasks":
                    {
                        ctx.ChangeState<TaskListState>();
                        update.Message.Text = null;
                        await ctx.HandleUpdateAsync(bot, update);
                        return;
                    }
                }
                await OnMessage(ctx, bot, update);
                break;

                case UpdateType.CallbackQuery:
                await OnCallbackQuery(ctx, bot, update);
                break;
            }
        }

        public virtual Task OnMessage(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update) => Task.CompletedTask;

        public virtual Task OnCallbackQuery(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update) => Task.CompletedTask;

        protected async Task ClearForm(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            if (ctx.Data.FormMsgId != null)
            {
                await bot.DeleteMessage(ctx.Data.ChatId, ctx.Data.FormMsgId.Value);
                ctx.Data.FormMsgId = null;
            }
        }
    }
}
