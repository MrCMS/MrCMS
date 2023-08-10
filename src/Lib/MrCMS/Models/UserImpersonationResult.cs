using MrCMS.Entities.People;

namespace MrCMS.Models;

public class UserImpersonationResult
{
    public bool Success => string.IsNullOrWhiteSpace(Error);
    public string Error { get; }
    public User UnderlyingUser { get; }

    private UserImpersonationResult(string error, User underlyingUser)
    {
        Error = error;
        UnderlyingUser = underlyingUser;
    }

    public static UserImpersonationResult SuccessResult(User underlyingUser)
    {
        return new UserImpersonationResult(null, underlyingUser);
    }

    public static UserImpersonationResult ErrorResult(string error)
    {
        return new UserImpersonationResult(error, null);
    }
}