using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Abstract;

/// <summary>
/// this class we use for handling flow in which can enter user 
/// </summary>
public interface IScenarios
{
    Task<Message> HandleAsync(Message message, UserState user);
    Task<Message> HandleCallbackAsync(CallbackQuery callback, UserState userState);
}