using Insurance.App.Enums;
using Insurance.App.Interface;
using Insurance.App.Scenarios;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Insurance.App.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger, IUserState userState, IScenarios senarios) : IUpdateHandler
{
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnMessage(message),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
    
        var currentState = userState.GetState(msg.Chat);
    
        if (currentState != UserState.None)
        {
            if (msg.Text is { } text)
            {
                var command = text.Split(' ')[0];
                
                if (command == "/butLicenceFlow")
                {
                    await LicenceFlow(msg, currentState);
                    return;
                }
            }
    
            await LicenceFlow(msg, currentState);
            return;
        }
    
        if (msg.Text is not { } messageText)
        {
            await bot.SendMessage(msg.Chat.Id, "Я розумію лише текстові команди. Наприклад: /start або /message");
            return;
        }
    
        var commandText = messageText.Split(' ')[0];
    
        var sentMessage = await (commandText switch
        {
            "/message" => SendMessageBack(msg),
            "/start" => SendStartMessage(msg),
            _ => Usage(msg)
        });
    
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }


    private Task<Message> LicenceFlow(Message msg, UserState userS)
    {
        return senarios.HandleAsync(msg, userS);
    }


    async Task<Message> SendStartMessage(Message msg)
    {
        const string usage = """
                             <b>Hi</b>
                             I'm insurance bot and help you with purchase
                             of licince or something like this 
                             """;

        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html);
    }

    async Task<Message> SendMessageBack(Message msg)
    {
        const string usage = "<b>Your are a stupid bitch</b>";

        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html);
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                                 <b><u>Bot menu</u></b>:
                                 /photo          - send out logo
                                 /message        - send some message
                                 /instructions   - for instructions
                             """;
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
    
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

}