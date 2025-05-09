using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Abstract;

//this is interface for
//manipulation with user state
public interface IUserState
{
    UserState GetState(Chat chat);
    void SetState(Chat chat, UserState state);
    void ResetState(Chat chat);
    void AddPhoto(Chat chat, PhotoSize photoBytes);
    List<PhotoSize> GetPhotos(Chat chat);
}