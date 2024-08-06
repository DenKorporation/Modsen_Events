namespace Domain.Constants;

public static class Policies
{
    public const string ApiScope = nameof(ApiScope);
    public const string ReadEvent = nameof(ReadEvent);
    public const string ModifyEvent = nameof(ModifyEvent);
    public const string UserIdMatching = nameof(UserIdMatching);
    public const string AdminRole = nameof(AdminRole);
    public const string UserIdMatchingOrAdminRole = nameof(UserIdMatchingOrAdminRole);
}