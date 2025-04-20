using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Interface;



public class UserStateService : IUserState
{
    // краще використовувати chat.Id замість Chat
    private readonly Dictionary<long, UserState> _userStates = new();

    public UserState GetState(Chat chat)
    {
        if (_userStates.TryGetValue(chat.Id, out var state))
        {
            return state;
        }

        return UserState.Default; 
    }

    public void SetState(Chat chat, UserState state)
    {
        _userStates[chat.Id] = state;
    }

    public void ResetState(Chat chat)
    {
        _userStates.Remove(chat.Id);
    }
}