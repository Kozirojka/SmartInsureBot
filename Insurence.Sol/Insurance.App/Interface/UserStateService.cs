using Insurance.App.Enums;
using Telegram.Bot.Types;

namespace Insurance.App.Interface;



public class UserStateService : IUserState
{
    // краще використовувати chat.Id замість Chat
    private readonly Dictionary<long, UserState> _userStates = new();
    private readonly Dictionary<long, List<PhotoSize>> _photos = new();
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
    public void AddPhoto(Chat chat, PhotoSize photoBytes)
    {
        if (!_photos.ContainsKey(chat.Id))
            _photos[chat.Id] = new List<PhotoSize>();

        var list = _photos[chat.Id];
        if (list.Count >= 2)
            list.RemoveAt(0); 

        list.Add(photoBytes);
    }
    
    public List<PhotoSize> GetPhotos(Chat chat)
    {
        return _photos.TryGetValue(chat.Id, out var list) ? list : new List<PhotoSize>();
    }
}