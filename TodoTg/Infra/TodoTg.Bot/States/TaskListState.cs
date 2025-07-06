using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TodoTg.Application.Services.Abstractions;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class TaskListState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        private const string TaskKey = "task";

        private const int PageSize = 5;

        public override async Task HandleUpdateAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            await base.HandleUpdateAsync(ctx, bot, update);
            if (ctx.IsUpdateHandled) return;

            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await ShowTaskList(ctx, bot, 1);
                    break;
                }
                case UpdateType.CallbackQuery:
                {
                    var cq = update.CallbackQuery!;
                    switch (cq.Key())
                    {
                        case TaskKey:
                        {
                            var id = cq.Value<Guid>();
                            await bot.AnswerCallbackQuery(cq.Id, $"You selected '{id}' task");
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
                    break;
                }
            }
        }

        private async Task ShowTaskList(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, int page, int? msgId = null)
        {
            var tasks = await todoService.ListAsync(ctx.Data.GetUserId(), page, PageSize);

            var buttons = tasks.Items.Select(todo => TgButton.Create(todo.Title, TaskKey, todo.Id)).ToList();
            var keyboard = TgPagination.Create(buttons, page, tasks.NumPages);

            if (msgId == null)
            {
                await bot.SendMessage(ctx.Data.ChatId, "Your tasks:", replyMarkup: keyboard);
            }
            else
            {
                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, msgId.Value, keyboard);
            }
        }
    }
}
