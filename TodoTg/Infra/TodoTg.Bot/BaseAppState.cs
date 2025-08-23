using System.Globalization;
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

        public virtual Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot) => Task.CompletedTask;

        public async Task OnUpdateAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            await update.CallbackQuery.AnswerIfEmpty(bot);
            ctx.Data.UserId ??= await UserService.EnsureCreatedForTelegram(new() { Name = ctx.Name, TgChatId = ctx.Data.ChatId });

            var prevCulture = CultureInfo.CurrentCulture;
            var prevUICulture = CultureInfo.CurrentUICulture;

            try
            {
                var userCulture = GetUserCulture(ctx);
                (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture) = (userCulture, userCulture);
                await HandleUpdateAsync(update, ctx, bot);
            }
            finally
            {
                (CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture) = (prevCulture, prevUICulture);
            }
        }

        public async Task HandleUpdateAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                switch (update.Message?.Text)
                {
                    case "/start":
                    {
                        await ClearForm(ctx, bot);
                        await ctx.ChangeState<StartState>();
                        return;
                    }
                    case "/newtask":
                    {
                        await ClearForm(ctx, bot);
                        await ctx.ChangeState<CreateTaskState>();
                        return;
                    }
                    case "/tasks":
                    {
                        await ClearForm(ctx, bot);
                        await ctx.ChangeState<TaskListState>();
                        return;
                    }
                }
                await OnMessageAsync(update, ctx, bot);
                break;

                case UpdateType.CallbackQuery:
                await OnCallbackAsync(update, ctx, bot);
                break;
            }
        }

        public virtual Task OnMessageAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot) => Task.CompletedTask;

        public virtual Task OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot) => Task.CompletedTask;

        protected static async Task ClearForm(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            if (ctx.Data.FormMsgId != null)
            {
                await bot.DeleteMessage(ctx.Data.ChatId, ctx.Data.FormMsgId.Value);
                ctx.Data.FormMsgId = null;
            }
        }

        protected static CultureInfo GetUserCulture(ChatContext<TgBotChatData> ctx)
        {
            return new CultureInfo(ctx.Data.Language switch
            {
                Language.Ru => "ru",
                _ => "en"
            });
        }
    }
}
