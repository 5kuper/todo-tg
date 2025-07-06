using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Utilities.TelegramBots.ContextProviders;
using Utilities.TelegramBots.StateMachine;

namespace Utilities.TelegramBots
{
    public class TgBotHostedService<TChatData>
        (IOptions<TgBotOptions> opt, ILogger<TgBotHostedService<TChatData>> logger, IContextProvider<TChatData> ctxAccessor)

        : BackgroundService where TChatData : IChatData
    {
        private readonly TelegramBotClient _bot = new(opt.Value.ApiKey);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: stoppingToken);
            logger.LogInformation("Bot started.");
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            var chat = update.Type switch
            {
                UpdateType.Message => update.Message?.Chat,
                UpdateType.CallbackQuery => update.CallbackQuery!.Message?.Chat,
                _ => null
            };

            if (chat != null)
            {
                var ctx = await ctxAccessor.GetAsync(chat.Id);
                ctx.Name = chat.FirstName + chat.LastName;
                await ctx.HandleUpdateAsync(bot, update);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken ct)
        {
            logger.LogError("Bot error: {err}", ex);
            return Task.CompletedTask;
        }
    }
}
