using Telegram.Bot;
using Utilities.TelegramBots.StateMachine;

namespace Utilities.TelegramBots.ContextProviders
{
    public interface IContextProvider<TChatData> where TChatData : IChatData
    {
        Task<ChatContext<TChatData>> GetAsync(long id, ITelegramBotClient bot);
    }
}
