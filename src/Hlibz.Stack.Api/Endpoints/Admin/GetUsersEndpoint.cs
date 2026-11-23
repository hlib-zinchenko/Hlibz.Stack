using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.GetUsers;

namespace Hlibz.Stack.Api.Endpoints.Admin;

/// <summary>Lists every user.</summary>
internal sealed class GetUsersEndpoint : IEndpoint<AdminUsersEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet(string.Empty, HandleAsync).WithSummary("Lists users.");

    private static Task<Result<PagedData<UserDto>>> HandleAsync(
        [AsParameters] UsersParameters parameters,
        [FromServices] IGetUsersQueryHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(parameters.ToQuery(), cancellationToken);
}
