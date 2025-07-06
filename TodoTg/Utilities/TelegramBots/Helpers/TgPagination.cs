using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.TelegramBots.Helpers
{
    public static class TgPagination
    {
        public const string Key = "page";

        public static InlineKeyboardMarkup Create(IList<InlineKeyboardButton> items, int currPage, int numPages)
        {
            if (numPages <= 0)
                throw new ArgumentOutOfRangeException(nameof(numPages), "Number of pages must be greater than zero.");

            if (currPage < 1 || currPage > numPages)
                throw new ArgumentOutOfRangeException(nameof(currPage), "Current page must be between 1 and the total number of pages.");

            var rows = items.Select(b => new[] { b }).ToList();
            var paginationRow = new List<InlineKeyboardButton>();

            if (currPage > 1)
                paginationRow.Add(TgButton.Create("◀️", Key, currPage - 1));

            paginationRow.Add(TgButton.Create($"{currPage}/{numPages}"));

            if (currPage < numPages)
                paginationRow.Add(TgButton.Create("▶️", Key, currPage + 1));

            rows.Add([..paginationRow]);
            return new InlineKeyboardMarkup(rows);
        }
    }
}
