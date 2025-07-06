using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utilities.TelegramBots.StateMachine
{
    public interface IChatData
    {
        public long ChatId { get; set; }
    }

    public class ChatContext<TData>(IBotState<TData> state, TData data, IServiceProvider sp) where TData : IChatData
    {
        private IBotState<TData> _state = state;

        public TData Data { get; } = data;

        public string Name { get; set; } = "";

        public bool IsUpdateHandled { get; set; }

        public void ChangeState<TState>() where TState : IBotState<TData>
        {
            var scope = sp.CreateScope();
            _state = scope.ServiceProvider.GetRequiredService<TState>();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update)
        {
            await _state.HandleUpdateAsync(this, bot, update);
        }
    }
}
