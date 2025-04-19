using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Scenarios;

public interface IScenarios
{
    Task<Message> HandleAsync(Message message, UserState user);
}