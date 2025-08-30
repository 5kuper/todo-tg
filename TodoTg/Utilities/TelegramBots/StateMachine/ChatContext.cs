using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utilities.TelegramBots.StateMachine
{
    public interface IChatData
    {
        public long ChatId { get; set; }
    }

    public class ChatContext<TData>(ITelegramBotClient bot, TData data, IServiceProvider sp) : IDisposable
        where TData : IChatData
    {
        private readonly SemaphoreSlim _gate = new(1, 1);

        private Type? _currentStateType;
        private IServiceScope? _updateScope;

        public string Name { get; set; } = "";

        public TData Data { get; } = data;

        public async Task HandleUpdateAsync(Update update)
        {
            await _gate.WaitAsync();
            try
            {
                _updateScope = sp.CreateScope();

                var state = _currentStateType != null
                    ? InstantiateCurrentState()
                    : await UseState<IDefaultState<TData>>();

                await state.OnUpdateAsync(update, this, bot);
            }
            finally
            {
                _updateScope?.Dispose();
                _updateScope = null;

                _gate.Release();
            }
        }

        public Task ChangeStateToDefault() => ChangeState<IDefaultState<TData>>();

        public async Task ChangeState<TState>() where TState : IBotState<TData>
        {
            await UseState<TState>();
        }

        private async Task<IBotState<TData>> UseState<TState>() where TState : IBotState<TData>
        {
            _currentStateType = typeof(TState);
            var state = InstantiateCurrentState();
            await state.OnEnterAsync(this, bot);
            return state;
        }

        private IBotState<TData> InstantiateCurrentState()
        {
            if (_updateScope is null || _currentStateType is null)
                throw new InvalidOperationException("Out of update scope.");

            return (IBotState<TData>)_updateScope.ServiceProvider.GetRequiredService(_currentStateType);
        }

        public void Dispose() => _gate.Dispose();
    }
}
