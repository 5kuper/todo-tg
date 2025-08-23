using System.Collections.Concurrent;
using Telegram.Bot;
using Utilities.TelegramBots.StateMachine;

namespace Utilities.TelegramBots.ContextProviders
{
    public class InMemoryContextProvider<TChatData>(IServiceProvider services) : IContextProvider<TChatData>
        where TChatData : IChatData, new()
    {
        private readonly ConcurrentDictionary<long, ChatContext<TChatData>> _dic = [];

        public Task<ChatContext<TChatData>> GetAsync(long id, ITelegramBotClient bot)
        {
            var ctx = _dic.GetOrAdd(id, id => new ChatContext<TChatData>(bot, new TChatData { ChatId = id }, services));
            return Task.FromResult(ctx);
        }
    }
}
