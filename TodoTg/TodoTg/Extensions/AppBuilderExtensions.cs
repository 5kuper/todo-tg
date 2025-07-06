using Microsoft.EntityFrameworkCore;
using TodoTg.Data;

namespace TodoTg.Extensions
{
    public static class AppBuilderExtensions
    {
        public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddEnvironmentVariables();
            builder.Configuration.AddUserSecrets<Program>();

            return builder;
        }

        public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
        {
            var connectionStr = builder.Configuration.GetConnectionString("PostgreSQL");
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionStr));

            return builder;
        }
    }
}
