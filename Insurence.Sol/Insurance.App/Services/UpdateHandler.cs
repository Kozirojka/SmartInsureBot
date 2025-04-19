using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Insurance.App.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError: {Exception}", exception);
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message }                        => OnMessage(message),
            { EditedMessage: { } message }                  => OnMessage(message),
            _                                               => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        logger.LogInformation("Receive message type: {MessageType}", msg.Type);
    
        switch (msg.Type)
        {
            case MessageType.Text:
                await HandleTextMessage(msg);
                break;
    
            case MessageType.Photo:
                await HandlePhotoMessage(msg);
                break;
    
            default:
                await Usage(msg);
                break;
        }
    }

    private async Task HandleTextMessage(Message msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Text))
            return;

        string command = msg.Text.Split(' ')[0].ToLowerInvariant();
    
        var response = command switch
        {
            "/message" => await SendMessageBack(msg),
            "/start" => await SendStartMessage(msg),
            "/instructions" => await SendInstructionInlineButton(msg),
            _ => await Usage(msg)
        };

        logger.LogInformation("Sent message with id: {MessageId}", response.MessageId);
    }

    private async Task HandlePhotoMessage(Message msg)
    {
        var sentMessage = await SendPhoto(msg);
        logger.LogInformation("Photo handled. MessageId: {MessageId}", sentMessage.MessageId);
    }
    async Task<Message> SendInstructionInlineButton(Message msg)
    {
        var instruction = new InlineKeyboardMarkup()
            .AddNewRow().AddButton("Instructions").AddButton("Proceed insurence");
        
        return await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: instruction);
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
        const string usage = "<b>Your are a stupud bitch</b>";

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
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); 
        await using var fileStream = new FileStream("D:\\development\\telegramBot@\\Insurence.Sol\\Insurance.App\\Files\\bot2.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }



    private Task UnknownUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
