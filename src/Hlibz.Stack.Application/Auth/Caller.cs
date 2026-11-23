namespace Hlibz.Stack.Application.Auth;

/// <summary>The signed-in user running a use case, with their role as stored now.</summary>
/// <param name="Id">The user's id.</param>
/// <param name="Role">The user's role.</param>
public sealed record Caller(UserId Id, Role Role)
{
    /// <summary>Gets a value indicating whether the caller is an admin.</summary>
    public bool IsAdmin => Role == Role.Admin;

    /// <summary>Gets whether the caller may manage what <paramref name="ownerId"/> owns.</summary>
    /// <param name="ownerId">The owner.</param>
    /// <returns><see langword="true"/> for the owner and for admins.</returns>
    public bool CanManage(UserId ownerId) => IsAdmin || ownerId == Id;
}
