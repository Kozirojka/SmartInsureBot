using Insurance.App.Abstract;
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

//todo: потрібно ще буде створити фабрику сценаріїв, для того, щоб користувач міг 
public class UpdateHandler(
    ITelegramBotClient bot,
    ILogger<UpdateHandler> logger,
    IUserState userState,
    IScenarios scenarios)
    : IUpdateHandler
{
    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient, 
        Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            await (update switch
            {
                { Message: { } message } => OnMessage(message),
                { EditedMessage: { } message } => OnMessage(message),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update {UpdateId}", update.Id);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task HandlePollingErrorAsync(
        ITelegramBotClient botClient, 
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Polling error");
        return Task.CompletedTask;
    }

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        var currentState = userState.GetState(callbackQuery.Message?.Chat);

        if (currentState != UserState.None)
        {
            await scenarios.HandleCallbackAsync(callbackQuery, currentState);
        }
    }
    
    private async Task OnMessage(Message message)
    {
        logger.LogInformation("Received message type: {MessageType}", message.Type);
    
        var currentState = userState.GetState(message.Chat);
        
        if (currentState != UserState.None)
        {
            if (message.Text is { } text && text.StartsWith("/"))
            {
                var command = text.Split(' ')[0];
                
                if (command == "/butLicenceFlow")
                {
                    await scenarios.HandleAsync(message, currentState);
                    return;
                }
                
                if (command == "/start" || command == "/restart")
                {
                    userState.SetState(message.Chat, UserState.Default);
                    await SendStartMessage(message);
                    return;
                }
            }
            
            await scenarios.HandleAsync(message, currentState);
            return;
        }
    
        if (message.Text is not { } messageText)
        {
            await bot.SendMessage(
                 message.Chat.Id, 
                 "I only understand text commands. For example: /start"
            );
            return;
        }
    
        var commandText = messageText.Split(' ')[0];
    
        var sentMessage = await (commandText switch
        {
            "/start" => SendStartMessage(message),
            _ => SendUsageMessage(message)
        });
    
        logger.LogInformation("Message sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> SendStartMessage(Message message)
    {
        const string startMessage = """
                             <b>Hi</b>
                             I'm an insurance bot and can help you with purchasing 
                             a license or related services.
                             
                             Type /butLicenceFlow to start the process.
                             """;

        var result = await bot.SendMessage(
             message.Chat.Id, 
             startMessage, 
            parseMode: ParseMode.Html
        );
        
        userState.SetState(message.Chat, UserState.Default);
        
        return result;
    }

    private async Task<Message> SendUsageMessage(Message message)
    {
        const string usageMessage = """
                                 <b><u>Bot menu</u></b>:
                                 /start         - Start or restart the bot
                                 /butLicenceFlow - Begin the license purchase process
                                 """;
                                 
        return await bot.SendMessage(  
             message.Chat.Id, 
             usageMessage, 
             ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }
    
    private Task<Message> UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.FromResult<Message>(null);
    }
}