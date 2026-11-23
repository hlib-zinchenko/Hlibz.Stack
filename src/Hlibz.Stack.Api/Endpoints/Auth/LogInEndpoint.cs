using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.LogIn;

namespace Hlibz.Stack.Api.Endpoints.Auth;

/// <summary>
/// Signs a user in. Send the token as <c>Authorization: Bearer {token}</c>, here and to the MCP
/// server.
/// </summary>
internal sealed class LogInEndpoint : IEndpoint<AuthEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPost("login", HandleAsync)
            .WithSummary("Logs in.")
            .ProducesErrors(StatusCodes.Status401Unauthorized);

    private static Task<Result<AccessTokenDto>> HandleAsync(
        LogInCommand command,
        [FromServices] ILogInCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(command, cancellationToken);
}
