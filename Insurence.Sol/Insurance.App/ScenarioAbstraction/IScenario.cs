using Telegram.Bot.Types;

namespace Insurance.App.Scenario;

public interface IScenario
{
    Task HandleAsync(Message message, CancellationToken cancellationToken);
    UserState State { get; }
}