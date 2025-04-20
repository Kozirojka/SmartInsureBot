using Insurance.App;
using Insurance.App.Interface;
using Insurance.App.Scenarios;
using Insurance.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mindee.Extensions.DependencyInjection;
using Telegram.Bot;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(context.Configuration.GetSection("BotConfiguration"));

        services.AddHttpClient("telegram_bot_client").RemoveAllLoggers()
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


        services.AddSingleton<IUserState, UserStateService>();
        services.AddSingleton<IScenarios, CreateLicenceScenarios>();
        services.AddSingleton<IPhotoService, PhotoService>();
        
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
        services.AddMindeeClient();
    })
    .Build();

await host.RunAsync();