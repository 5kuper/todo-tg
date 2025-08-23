using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TodoTg.Application.Services.Abstractions;
using TodoTg.Bot.Resources;
using Utilities.TelegramBots.Helpers;
using Utilities.TelegramBots.StateMachine;

namespace TodoTg.Bot.States
{
    public class StartState(IUserService userService) : BaseAppState(userService)
    {
        private const string LangKey = "lang";

        public override async Task OnEnterAsync(ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var keyboard = new InlineKeyboardMarkup(new[]
{
                TgButton.CreateForEnum("English", LangKey, Language.En).Row(),
                TgButton.CreateForEnum("Русский", LangKey, Language.Ru).Row()
            });

            var msg = await bot.SendMessage(ctx.Data.ChatId, Strings.ChooseLanguage, replyMarkup: keyboard);
            ctx.Data.FormMsgId = msg.MessageId;
        }

        public override async Task OnCallbackAsync(Update update, ChatContext<TgBotChatData> ctx, ITelegramBotClient bot)
        {
            var cq = update.CallbackQuery!;
            if (cq.Key() == LangKey)
            {
                ctx.Data.Language = cq.EnumValue<Language>();
                var text = Strings.ResourceManager.GetString(nameof(Strings.LangUpdated), GetUserCulture(ctx));

                await bot.EditMessageReplyMarkup(ctx.Data.ChatId, cq.Message!.Id, replyMarkup: null);
                await bot.EditMessageText(ctx.Data.ChatId, cq.Message!.Id, text!);

                ctx.Data.FormMsgId = null;

                await bot.AnswerCallbackQuery(cq.Id);
                await ctx.ChangeStateToDefault();
                return;
            }
        }
    }
}
