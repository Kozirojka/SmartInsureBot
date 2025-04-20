using Insurance.App.Abstract;
using Telegram.Bot.Types;

namespace Insurance.App.Services;

public class CommandInterpreterService : ICommandInterpreterService
{
    public Task<string> InterpretCommand(Message message)
    {
        throw new NotImplementedException();
    }
}