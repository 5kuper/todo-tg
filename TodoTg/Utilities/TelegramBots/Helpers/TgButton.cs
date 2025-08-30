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

        public static InlineKeyboardButton CreateForEnum<T>(string text, string key, T value)
            where T : struct, Enum
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

        public static string Value(this CallbackQuery cq)
        {
            if (string.IsNullOrWhiteSpace(cq.Data))
                throw new FormatException("Empty callback data.");

            var i = cq.Data.IndexOf(CallbackSeparator);
            if (i < 0 || i == cq.Data.Length - 1)
                throw new FormatException("No value in the callback data.");

            return cq.Data[(i + 1)..];
        }

        public static T Value<T>(this CallbackQuery cq)
            where T : IParsable<T>
        {
            var isParsed = T.TryParse(cq.Value(), null, out var result);

            return isParsed && result != null ? result
                : throw new FormatException("Invalid callback data.");
        }

        public static T EnumValue<T>(this CallbackQuery cq)
            where T : struct, Enum
        {
            var isParsed = Enum.TryParse<T>(cq.Value(), true, out var result);
            return isParsed ? result : throw new FormatException("Invalid callback data.");
        }

        public static InlineKeyboardButton[] Row(this InlineKeyboardButton btn) => [btn];

        public static async Task<bool> AnswerIfEmpty(this CallbackQuery? cq, ITelegramBotClient bot)
        {
            if (cq?.Key() == EmptyKey)
            {
                await bot.AnswerCallbackQuery(cq.Id);
                return true;
            }
            return false;
        }
    }
}
