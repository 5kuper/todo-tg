using TodoTg;
using TodoTg.Bot;
using TodoTg.Bot.States;
using TodoTg.Extensions;
using Utilities.TelegramBots;

var builder = WebApplication.CreateBuilder(args);

builder.AddOptions();
builder.AddDatabase();

builder.AddTelegramBot<TgBotChatData, DefaultState>();

builder.Services.AddCoreServices();
builder.Services.AddRepositories();

builder.Services.AddTgBotStates();
builder.Services.AddWebApi();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
