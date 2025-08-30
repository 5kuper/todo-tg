using TodoTg.Application.Services.Abstractions;
using TodoTg.Application.Services.Implementations;
using TodoTg.Bot.States;
using TodoTg.Data.Repositories;
using TodoTg.Domain.Repositories;

namespace TodoTg.Extensions
{
    public static class ServicesExtentions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITodoService, TodoService>();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();

            return services;
        }

        public static IServiceCollection AddTgBotStates(this IServiceCollection services)
        {
            services.AddScoped<StartState>();

            services.AddScoped<CreateTaskState>();
            services.AddScoped<TaskListState>();
            services.AddScoped<TaskInfoState>();

            return services;
        }

        public static IServiceCollection AddWebApi(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }
    }
}
