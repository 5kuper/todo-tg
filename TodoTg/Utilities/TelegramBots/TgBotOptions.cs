using System.ComponentModel.DataAnnotations;

namespace Utilities.TelegramBots
{
    public class TgBotOptions
    {
        [Required]
        public string ApiKey { get; set; } = null!;
    }
}
