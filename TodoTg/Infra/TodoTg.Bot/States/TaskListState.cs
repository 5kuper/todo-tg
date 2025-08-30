using Telegram.Bot;
using Telegram.Bot.Types;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;
using Utilities.Text;

namespace TodoTg.Bot.States
{
    public class TaskListState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        private const string TaskKey = "task";

        private const int PageSize = 5;

        public override async Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            await ShowTaskList(ctx, bot, ctx.Data.CurrentTasksPage);
        }

        public override async Task<bool> OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var cq = update.CallbackQuery!;
            switch (cq.Key())
            {
                case TaskKey:
                {
                    ctx.Data.SelectedTaskId = cq.Value<Guid>();
                    await bot.DeleteMessage(ctx.Data.ChatId, cq.Message!.Id);
                    await ctx.ChangeState<TaskInfoState>();
                    return true;
                }
                case TgPagination.Key:
                {
                    var page = cq.Value<int>();
                    await ShowTaskList(ctx, bot, page, cq.Message!.Id);
                    return true;
                }
            }

            return false;
        }

        private async Task ShowTaskList(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, int page, int? msgId = null)
        {
            var tasks = await todoService.ListAsync(ctx.Data.GetUserId(), page, PageSize);

            if (tasks.TotalCount == 0)
            {
                await bot.SendMessage(ctx.Data.ChatId, Strings.NoTasks);
            }

            var buttons = tasks.Items.Select(todo =>
                TgButton.Create($"{(todo.IsCompleted ? "✅" : "🔲")} {todo.Title}", TaskKey, todo.Id)).ToList();

            var keyboard = TgPagination.Create(buttons, page, tasks.NumPages);

            if (msgId == null)
            {
                var msg = await bot.SendMessage(ctx.Data.ChatId, Strings.YourTasks.PadCenter(), replyMarkup: keyboard);
                ctx.Data.FormMsgId = msg.MessageId;
            }
            else
            {
                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, msgId.Value, keyboard);
            }
        }
    }
}
