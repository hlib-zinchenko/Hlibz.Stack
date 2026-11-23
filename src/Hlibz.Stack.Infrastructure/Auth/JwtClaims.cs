namespace Hlibz.Stack.Infrastructure.Auth;

/// <summary>The claims an access token carries.</summary>
public static class JwtClaims
{
    /// <summary>The user's id.</summary>
    public const string Subject = "sub";

    /// <summary>The user's name.</summary>
    public const string Name = "name";

    /// <summary>The user's email address.</summary>
    public const string Email = "email";
}
