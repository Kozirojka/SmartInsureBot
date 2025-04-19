using Console.Advanced.Abstract;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Insurance.App.Services;

public class ReceiverService(ITelegramBotClient botClient, UpdateHandler updateHandler, ILogger<ReceiverServiceBase<UpdateHandler>> logger)
    : ReceiverServiceBase<UpdateHandler>(botClient, updateHandler, logger);
