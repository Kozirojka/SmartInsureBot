using Console.Advanced.Abstract;
using Insurance.App.Abstract;
using Microsoft.Extensions.Logging;

namespace Insurance.App.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
