using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utilities.TelegramBots.StateMachine
{
    public interface IChatData
    {
        public long ChatId { get; set; }
    }

    public class ChatContext<TData>(TData data, IServiceProvider sp) where TData : IChatData
    {
        private Type? _stateType;

        public string Name { get; set; } = "";

        public TData Data { get; } = data;

        public void ChangeState<TState>() where TState : IBotState<TData> => _stateType = typeof(TState);

        public void ChangeStateToDefault() => _stateType = null;

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update)
        {
            using var scope = sp.CreateScope();

            var state = _stateType != null
                ? (IBotState<TData>)scope.ServiceProvider.GetRequiredService(_stateType)
                : scope.ServiceProvider.GetRequiredService<IDefaultState<TData>>();

            await state.HandleUpdateAsync(this, bot, update);
        }
    }
}
