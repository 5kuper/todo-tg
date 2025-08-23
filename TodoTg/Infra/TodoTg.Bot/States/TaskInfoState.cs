using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using TodoTg.Domain.ValueObj;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class TaskInfoState(IUserService userService, ITodoService todoService) : BaseAppState(userService)
    {
        private const string CompleteKey = "set-complated";

        private const string EditKey = "edit";

        private const string CancelKey = "cancel";

        public override async Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            await ShowInfo(ctx, bot);
        }

        public override async Task OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var cq = update.CallbackQuery!;
            switch (cq.Key())
            {
                case CompleteKey:
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
            }
        }

        public override async Task OnMessageAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
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
                        await bot.SendMessage(ctx.Data.ChatId, Strings.TitleTooShort);
                        return;
                    }

                    await todoService.UpdateAsync(ctx.Data.SelectedTaskId.Value, new() { Title = msg });
                    ctx.Data.SelectedEditOption = null;
                    await bot.SendMessage(ctx.Data.ChatId, Strings.TaskRenamed);
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
                await bot.SendMessage(ctx.Data.ChatId, Strings.NoTask);
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
                    {task.Description ?? Strings.NoDescription}
                    """;

            var btnText = task.IsCompleted ? Strings.CompletedBtn : Strings.UncompletedButton;
            var keyboard = new InlineKeyboardMarkup(new[]
            {
                TgButton.Create(btnText, CompleteKey, !task.IsCompleted).Row(),
                TgButton.Create(Strings.EditBtn, EditKey, 0).Row()
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

                    buttons.Add(TgButton.Create(Strings.CancelBtn, CancelKey, 0).Row());
                    var keyboard = new InlineKeyboardMarkup(buttons);

                    ctx.Data.SelectedEditOption = null;

                    await bot.EditMessageText(ctx.Data.ChatId, cq.Message!.Id, Strings.WhatToEdit);
                    await bot.EditMessageReplyMarkup(ctx.Data.ChatId, cq.Message!.Id, keyboard);

                    await bot.AnswerCallbackQuery(cq.Id);
                    return;
                }
                case nameof(EditOption.Title):
                {
                    await ClearForm(ctx, bot);

                    ctx.Data.SelectedEditOption = EditOption.Title;
                    await bot.SendMessage(ctx.Data.ChatId, Strings.EnterNewTaskTitle);

                    await bot.AnswerCallbackQuery(cq.Id);
                    return;
                }
            }
        }
    }
}
