using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.GetCurrentUser;

namespace Hlibz.Stack.Api.Endpoints.Users;

/// <summary>Gets the signed-in user.</summary>
internal sealed class GetCurrentUserEndpoint : IEndpoint<UsersEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet("me", HandleAsync)
            .WithSummary("Gets you.")
            .ProducesErrors(StatusCodes.Status404NotFound);

    private static Task<Result<UserDto>> HandleAsync(
        [FromServices] IGetCurrentUserQueryHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new GetCurrentUserQuery(), cancellationToken);
}
