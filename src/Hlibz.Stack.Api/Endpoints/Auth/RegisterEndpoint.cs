using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.RegisterUser;

namespace Hlibz.Stack.Api.Endpoints.Auth;

/// <summary>Signs a new user up. Log in afterwards for an access token.</summary>
internal sealed class RegisterEndpoint : IEndpoint<AuthEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPost("register", HandleAsync)
            .WithSummary("Signs up.")
            .ProducesErrors(StatusCodes.Status409Conflict);

    private static Task<Result<UserDto>> HandleAsync(
        RegisterUserCommand command,
        [FromServices] IRegisterUserCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(command, cancellationToken);
}
