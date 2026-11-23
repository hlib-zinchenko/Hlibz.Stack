namespace Hlibz.Stack.Application.Auth;

/// <summary>
/// Who the host says is calling: the Api and the Mcp host read the signed-in user's id from the
/// validated access token.
/// </summary>
public interface ICurrentUser
{
    /// <summary>Gets the signed-in user's id; <see langword="null"/> for an anonymous caller.</summary>
    UserId? Id { get; }
}
