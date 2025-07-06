using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utilities.TelegramBots.StateMachine
{
    public interface IDefaultState<TChatData> : IBotState<TChatData> where TChatData : IChatData;

    public interface IBotState<TChatData> where TChatData : IChatData
    {
        Task HandleUpdateAsync(ChatContext<TChatData> ctx, ITelegramBotClient bot, Update update);
    }
}
