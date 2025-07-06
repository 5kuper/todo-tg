using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Utilities.TelegramBots.ContextProviders;
using Utilities.TelegramBots.StateMachine;

namespace Utilities.TelegramBots
{
    public static class AppBuilderExtensions
    {
        public static WebApplicationBuilder AddTelegramBot<TChatData, TDefaultState, TContextProvider>
            (this WebApplicationBuilder builder, ServiceLifetime ctxProviderLifetime = ServiceLifetime.Scoped)

            where TChatData : IChatData, new()
            where TDefaultState : class, IDefaultState<TChatData>
            where TContextProvider : IContextProvider<TChatData>
        {
            builder.Services.AddOptions<TgBotOptions>().BindConfiguration(nameof(TgBotOptions))
                .ValidateDataAnnotations().ValidateOnStart();

            builder.Services.Add(new ServiceDescriptor(typeof(IContextProvider<TChatData>),
                typeof(TContextProvider), ctxProviderLifetime));

            builder.Services.AddScoped<IDefaultState<TChatData>, TDefaultState>();
            builder.Services.AddScoped<TDefaultState>();

            builder.Services.AddHostedService<TgBotHostedService<TChatData>>();
            return builder;
        }

        public static WebApplicationBuilder AddTelegramBot<TChatData, TStartState>(this WebApplicationBuilder builder)
            where TChatData : IChatData, new()
            where TStartState : class, IDefaultState<TChatData>
        {
            return builder.AddTelegramBot<TChatData, TStartState, InMemoryContextProvider<TChatData>>(ServiceLifetime.Singleton);
        }
    }
}
