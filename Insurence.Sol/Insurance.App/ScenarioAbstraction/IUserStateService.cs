namespace Insurance.App.Scenario;

public interface IUserStateService
{
    UserState? GetUserState(long userId);
    void SetUserState(long userId, UserState state);
    void ClearUserState(long userId);
}
