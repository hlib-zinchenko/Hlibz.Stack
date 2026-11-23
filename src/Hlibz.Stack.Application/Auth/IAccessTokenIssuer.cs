using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.Auth;

/// <summary>Issues the access token a signed-in user sends to the Api and the Mcp host.</summary>
public interface IAccessTokenIssuer
{
    /// <summary>Issues a token for <paramref name="user"/>.</summary>
    /// <param name="user">The user who signed in.</param>
    /// <returns>The token.</returns>
    AccessTokenDto Issue(User user);
}
