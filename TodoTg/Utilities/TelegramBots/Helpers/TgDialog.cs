using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.TelegramBots.Helpers
{
    public static class TgDialog
    {
        public const string OkKey = "ok";

        public static InlineKeyboardButton CreateOkKeyboard()
        {
            return TgButton.Create("OK", OkKey, 0);
        }

        public static async Task<bool> AnswerIfDialog(this CallbackQuery? cq, ITelegramBotClient bot)
        {
            if (cq?.Key() == OkKey)
            {
                if (cq.Message != null)
                    await bot.DeleteMessage(cq.Message.Chat.Id, cq.Message.Id);

                await bot.AnswerCallbackQuery(cq.Id);
                return true;
            }
            return false;
        }
    }
}
