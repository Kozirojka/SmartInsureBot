namespace Insurance.App.Enums;

/// <summary>
/// This is enums for managing the state of the scenario 
/// </summary>
public enum UserState
{
    AwaitingPhotoPassport,
    AwaitingPhotoDrivingLicense,
    None,
    Default,
    AwaitingForApproving,
    AllowForApprove,
    OpenPurchesePipe
}