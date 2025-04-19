using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Interface;


//this is interface for
//manipulation with user state
public interface IUserState
{
    UserState GetState(Chat chat);
    void SetState(Chat chat, UserState state);
    void ResetState(Chat chat);
}
