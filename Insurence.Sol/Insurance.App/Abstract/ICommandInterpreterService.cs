using Telegram.Bot.Types;

namespace Insurance.App.Abstract;

public interface ICommandInterpreterService
{
    Task<string> InterpretCommand(Message message);
}