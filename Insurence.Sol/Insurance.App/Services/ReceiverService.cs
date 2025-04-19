using Console.Advanced.Abstract;
using Insurance.App.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Console.Advanced.Services;

public class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
    : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger);
