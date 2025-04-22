using Insurance.App;
using Insurance.App.Abstract;
using Insurance.App.Interface;
using Insurance.App.Scenarios;
using Insurance.App.Services;
using Microsoft.Extensions.Options;
using Mindee.Extensions.DependencyInjection;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<BotConfiguration>(
    builder.Configuration.GetSection("BotConfiguration"));

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        BotConfiguration? botConfiguration = sp.GetService<IOptions<BotConfiguration>>()?.Value;
        ArgumentNullException.ThrowIfNull(botConfiguration);

        if (string.IsNullOrWhiteSpace(botConfiguration.BotToken))
        {
            throw new InvalidOperationException("Telegram bot token is missing in configuration.");
        }

        TelegramBotClientOptions options = new(botConfiguration.BotToken);
        return new TelegramBotClient(options, httpClient);
    });

builder.Services.AddSingleton<IUserState, UserStateService>();
builder.Services.AddSingleton<IScenarios, CreateLicenceScenarios>();
builder.Services.AddSingleton<IPhotoService, PhotoService>();

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddMindeeClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => "Bot is running!");

app.Run();
