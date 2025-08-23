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
            await ShowTaskList(ctx, bot, 1);
        }

        public override async Task OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var cq = update.CallbackQuery!;
            switch (cq.Key())
            {
                case TaskKey:
                {
                    ctx.Data.SelectedTaskId = cq.Value<Guid>();

                    await bot.AnswerCallbackQuery(cq.Id);
                    await bot.DeleteMessage(ctx.Data.ChatId, cq.Message!.Id);

                    await ctx.ChangeState<TaskInfoState>();
                    break;
                }
                case TgPagination.Key:
                {
                    var page = cq.Value<int>();
                    await ShowTaskList(ctx, bot, page, cq.Message!.Id);
                    await bot.AnswerCallbackQuery(cq.Id);
                    break;
                }
            }
        }

        private async Task ShowTaskList(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, int page, int? msgId = null)
        {
            var tasks = await todoService.ListAsync(ctx.Data.GetUserId(), page, PageSize);

            var buttons = tasks.Items.Select(todo =>
                TgButton.Create($"{(todo.IsCompleted ? "✅" : "🔲")} {todo.Title}", TaskKey, todo.Id)).ToList();

            var keyboard = TgPagination.Create(buttons, page, tasks.NumPages);

            if (msgId == null)
            {
                var msg = await bot.SendMessage(ctx.Data.ChatId, Strings.YourTasks.PadCenter(50), replyMarkup: keyboard);
                ctx.Data.FormMsgId = msg.MessageId;
            }
            else
            {
                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, msgId.Value, keyboard);
            }
        }
    }
}
