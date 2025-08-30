using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using TodoTg.Domain.ValueObj;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class StartState(IUserService userService) : BaseAppState(userService)
    {
        private const string LangKey = "lang";

        public override async Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var keyboard = new InlineKeyboardMarkup([
                TgButton.CreateForEnum("English", LangKey, AppLanguage.En).Row(),
                TgButton.CreateForEnum("Русский", LangKey, AppLanguage.Ru).Row()
            ]);

            var msg = await bot.SendMessage(ctx.Data.ChatId, Strings.ChooseLanguage, replyMarkup: keyboard);
            ctx.Data.FormMsgId = msg.MessageId;
        }

        public override async Task<bool> OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var cq = update.CallbackQuery!;
            if (cq.Key() == LangKey)
            {
                var lang = cq.EnumValue<AppLanguage>();
                await UserService.UpdateAsync(ctx.Data.GetUserId(), new() { Language = lang });

                var text = Strings.ResourceManager.GetString(nameof(Strings.LangUpdated), LangToCulture(lang));
                await bot.EditMessageText(ctx.Data.ChatId, cq.Message!.Id, text!);

                var keyboard = TgDialog.CreateOkKeyboard();
                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, cq.Message!.Id, replyMarkup: keyboard);

                ctx.Data.FormMsgId = null;

                await ctx.ChangeStateToDefault();
                return true;
            }

            return false;
        }
    }
}
