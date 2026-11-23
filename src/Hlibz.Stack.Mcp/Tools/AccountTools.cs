using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.GetCurrentUser;

namespace Hlibz.Stack.Mcp.Tools;

/// <summary>The signed-in user.</summary>
[McpServerToolType]
internal static class AccountTools
{
    [McpServerTool(Name = "get_me", Title = "Who am I", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Gets the signed-in user: id, email, name and role.")]
    public static async Task<UserDto> GetMeAsync(
        IGetCurrentUserQueryHandler handler,
        CancellationToken cancellationToken) =>
        (await handler.HandleAsync(new GetCurrentUserQuery(), cancellationToken)).ValueOrThrow();
}
