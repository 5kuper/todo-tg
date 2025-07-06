using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.TelegramBots.Helpers
{
    public static class TgButton
    {
        private const char CallbackSeparator = ':';

        private const string EmptyKey = "empty";

        public static InlineKeyboardButton Create(string text) => Create(text, EmptyKey, 0);

        public static InlineKeyboardButton Create<T>(string text, string key, T value)
            where T : IParsable<T>
        {
            var data = $"{key}{CallbackSeparator}{value}";
            return InlineKeyboardButton.WithCallbackData(text, data);
        }

        public static string? Key(this CallbackQuery cq)
        {
            if (string.IsNullOrWhiteSpace(cq.Data)) return null;

            var i = cq.Data.IndexOf(CallbackSeparator);
            return i > 0 ? cq.Data[..i] : null;
        }

        public static T Value<T>(this CallbackQuery cq)
            where T : IParsable<T>
        {
            if (string.IsNullOrWhiteSpace(cq.Data))
                throw new FormatException("Empty callback data.");

            var i = cq.Data.IndexOf(CallbackSeparator);
            if (i < 0 || i == cq.Data.Length - 1)
                throw new FormatException("No value in the callback data.");

            var isParsed = T.TryParse(cq.Data[(i + 1)..], null, out var result);

            return isParsed && result != null ? result
                : throw new FormatException("Invalid callback data.");
        }

        public static async Task AnswerIfEmpty(this CallbackQuery? cq, ITelegramBotClient bot)
        {
            if (cq?.Key() == EmptyKey) await bot.AnswerCallbackQuery(cq.Id);
        }
    }
}
