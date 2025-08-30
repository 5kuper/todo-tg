using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using TodoTg.Bot.States;
using TodoTg.Domain.ValueObj;
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
            if (await update.CallbackQuery.AnswerIfEmpty(bot) ||
                await update.CallbackQuery.AnswerIfDialog(bot)) return;

            var user = await UserService.EnsureCreatedForTelegram(new() { Name = ctx.Name, TgChatId = ctx.Data.ChatId });
            ctx.Data.UserId ??= user.Id;

            var prevCulture = CultureInfo.CurrentCulture;
            var prevUICulture = CultureInfo.CurrentUICulture;

            try
            {
                var userCulture = LangToCulture(user.Language);
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
                if (update.Message?.Text is not null && update.Message.Text.StartsWith('/'))
                {
                    var name = update.Message.Text?.Split(' ')[0].TrimStart('/');

                    await ClearForm(ctx, bot);
                    switch (name)
                    {
                        case "start":
                        await ctx.ChangeState<StartState>();
                        break;

                        case "newtask":
                        await ctx.ChangeState<CreateTaskState>();
                        break;

                        case "tasks":
                        ctx.Data.CurrentTasksPage = 1;
                        await ctx.ChangeState<TaskListState>();
                        break;

                        default:
                        await bot.SendMessage(ctx.Data.ChatId, Strings.UnknownCommand, replyMarkup: TgDialog.CreateOkKeyboard());
                        break;
                    }
                    return;
                }
                else
                {
                    await OnMessageAsync(update, ctx, bot);
                }
                break;

                case UpdateType.CallbackQuery:
                {
                    var result = await OnCallbackAsync(update, ctx, bot);
                    var cq = update.CallbackQuery!;

                    if (!result)
                    {
                        if (cq.Message != null)
                            await bot.DeleteMessage(ctx.Data.ChatId, cq.Message.Id);

                        var keyboard = TgDialog.CreateOkKeyboard();
                        await bot.SendMessage(ctx.Data.ChatId, Strings.SessionExpired, replyMarkup: keyboard);
                    }

                    await bot.AnswerCallbackQuery(cq.Id);
                    break;
                }
            }
        }

        public virtual Task OnMessageAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot) => Task.CompletedTask;

        public virtual Task<bool> OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot) => Task.FromResult(false);

        protected static async Task ClearForm(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            if (ctx.Data.FormMsgId != null)
            {
                await bot.DeleteMessage(ctx.Data.ChatId, ctx.Data.FormMsgId.Value);
                ctx.Data.FormMsgId = null;
            }
        }

        protected static CultureInfo LangToCulture(AppLanguage lang)
        {
            return new CultureInfo(lang switch
            {
                AppLanguage.Ru => "ru",
                _ => "en"
            });
        }
    }
}
