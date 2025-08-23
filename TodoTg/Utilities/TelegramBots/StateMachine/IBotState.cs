using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utilities.TelegramBots.StateMachine
{
    public interface IDefaultState<TChatData> : IBotState<TChatData>
        where TChatData : IChatData;

    public interface IBotState<TChatData>
        where TChatData : IChatData
    {
        Task OnEnterAsync(ChatContext<TChatData> ctx, ITelegramBotClient bot);

        Task OnUpdateAsync(Update update, ChatContext<TChatData> ctx, ITelegramBotClient bot);
    }
}
