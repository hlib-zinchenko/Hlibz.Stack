using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Infrastructure.Auth;

namespace Hlibz.Stack.Mcp;

/// <summary>The user the MCP request's validated access token names.</summary>
/// <param name="accessor">The current request.</param>
internal sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public UserId? Id =>
        accessor.HttpContext?.User.FindFirst(JwtClaims.Subject)?.Value is { } subject
        && Guid.TryParse(subject, out Guid id)
            ? UserId.From(id)
            : null;
}
