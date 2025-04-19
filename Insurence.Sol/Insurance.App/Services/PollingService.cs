using Console.Advanced.Abstract;
using Console.Advanced.Services;
using Microsoft.Extensions.Logging;

namespace Insurance.App.Services;

public class PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
    : PollingServiceBase<ReceiverService>(serviceProvider, logger);
