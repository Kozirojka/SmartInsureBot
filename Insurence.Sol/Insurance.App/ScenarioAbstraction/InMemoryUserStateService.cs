namespace Insurance.App.Scenario;

public class InMemoryUserStateService : IUserStateService
{
    private readonly Dictionary<long, UserState> _userStates = new();

    public UserState? GetUserState(long userId)
        => _userStates.TryGetValue(userId, out var state) ? state : null;

    public void SetUserState(long userId, UserState state)
        => _userStates[userId] = state;

    public void ClearUserState(long userId)
        => _userStates.Remove(userId);
}
