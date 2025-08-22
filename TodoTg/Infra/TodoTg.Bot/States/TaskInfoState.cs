using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Domain.ValueObj;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class TaskInfoState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        private const string ComplateKey = "set-complated";

        private const string EditKey = "edit";

        private const string CancelKey = "cancel";

        public override async Task OnCallbackQuery(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            var cq = update.CallbackQuery!;
            switch (cq.Key())
            {
                case ComplateKey:
                {
                    if (ctx.Data.SelectedTaskId is null)
                        throw new InvalidOperationException("SelectedTaskId is null.");

                    await todoService.UpdateAsync(ctx.Data.SelectedTaskId.Value, new() { IsCompleted = cq.Value<bool>() });
                    await ShowInfo(ctx, bot, cq.Message!.Id, true);
                    return;
                }
                case EditKey:
                {
                    await HandleEditKey(ctx, bot, cq);
                    break;
                }
                case CancelKey:
                {
                    await ShowInfo(ctx, bot, cq.Message!.Id);
                    return;
                }
                default:
                {
                    await ShowInfo(ctx, bot);
                    return;
                }
            }
        }

        public override async Task OnMessage(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, Update update)
        {
            if (ctx.Data.SelectedEditOption is null) return;

            if (ctx.Data.SelectedTaskId is null)
                throw new InvalidOperationException("SelectedTaskId is null.");

            var msg = update.Message?.Text;
            switch (ctx.Data.SelectedEditOption)
            {
                case EditOption.Title:
                {
                    if (string.IsNullOrWhiteSpace(msg) || msg.Length < 3)
                    {
                        await bot.SendMessage(ctx.Data.ChatId, "The title is too short. Enter it again:");
                        return;
                    }

                    await todoService.UpdateAsync(ctx.Data.SelectedTaskId.Value, new() { Title = msg });
                    ctx.Data.SelectedEditOption = null;
                    await bot.SendMessage(ctx.Data.ChatId, "The task has been renamed.");
                    await ShowInfo(ctx, bot);
                    break;
                }
            }
        }

        private async Task ShowInfo(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot,
            int? msgId = null, bool editKeydoardOnly = false)
        {
            if (ctx.Data.SelectedTaskId is null)
                throw new InvalidOperationException("SelectedTaskId is null.");

            var task = await todoService.GetAsync(ctx.Data.SelectedTaskId.Value);
            if (task is null)
            {
                await bot.SendMessage(ctx.Data.ChatId, "No task");
                return;
            }

            var priorityMark = task.Priority switch
            {
                Priority.High => "🔴 ",
                Priority.Medium => "🟠 ",
                Priority.Low => "🟢 ",
                _ => ""
            };

            var text = $"""
                    {priorityMark}<b>{task.Title}</b>
                    {task.Description ?? "No description."}
                    """;

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                    TgButton.Create(task.IsCompleted ? "✅ Complated" : "🔲 Uncompleted", ComplateKey, !task.IsCompleted).Row(),
                    TgButton.Create("Edit", EditKey, 0).Row()
                });

            if (msgId is null)
            {
                var msg = await bot.SendMessage(ctx.Data.ChatId, text, ParseMode.Html, replyMarkup: keyboard);
                ctx.Data.FormMsgId = msg.MessageId;
            }
            else
            {
                if (!editKeydoardOnly)
                    await bot.EditMessageText(ctx.Data.ChatId, msgId.Value, text, ParseMode.Html);

                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, msgId.Value, keyboard);
            }
        }

        private async Task HandleEditKey(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot, CallbackQuery cq)
        {
            switch (cq.Value<string>())
            {
                case "0":
                {
                    var buttons = Enum.GetValues<EditOption>()
                        .Select(x => x.ToString())
                        .Select(x => TgButton.Create("▶️ " + x, EditKey, x).Row())
                        .ToList();

                    buttons.Add(TgButton.Create("❌ Cancel", CancelKey, 0).Row());
                    var keyboard = new InlineKeyboardMarkup(buttons);

                    ctx.Data.SelectedEditOption = null;

                    await bot.EditMessageText(ctx.Data.ChatId, cq.Message!.Id, "What do you want to edit?");
                    await bot.EditMessageReplyMarkup(ctx.Data.ChatId, cq.Message!.Id, keyboard);

                    await bot.AnswerCallbackQuery(cq.Id);
                    return;
                }
                case nameof(EditOption.Title):
                {
                    await ClearForm(ctx, bot);

                    ctx.Data.SelectedEditOption = EditOption.Title;
                    await bot.SendMessage(ctx.Data.ChatId, "Enter a new title:");

                    await bot.AnswerCallbackQuery(cq.Id);
                    return;
                }
            }
        }
    }
}
