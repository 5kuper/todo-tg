using Microsoft.Extensions.DependencyInjection;
using Utilities.TelegramBots.StateMachine;

namespace Utilities.TelegramBots.ContextProviders
{
    public class InMemoryContextProvider<TChatData>(IServiceProvider services) : IContextProvider<TChatData>
        where TChatData : IChatData, new()
    {
        private readonly Dictionary<long, ChatContext<TChatData>> _dic = [];

        public Task<ChatContext<TChatData>> GetAsync(long id)
        {
            var ctx = _dic.GetValueOrDefault(id);

            if (ctx == null)
            {
                var scope = services.CreateScope();
                var defaultState = scope.ServiceProvider.GetRequiredService<IDefaultState<TChatData>>();

                ctx = new(defaultState, new TChatData() { ChatId = id }, services);
                _dic[id] = ctx;
            }

            return Task.FromResult(ctx);
        }
    }
}
