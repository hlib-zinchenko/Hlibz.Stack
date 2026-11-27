using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

namespace Hlibz.Stack.Api.Endpoints.Admin;

/// <summary>Changes what a user may do. Admins can't change their own role.</summary>
internal sealed class ChangeUserRoleEndpoint : IEndpoint<AdminUsersEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPut("{userId}/role", HandleAsync)
            .WithSummary("Changes a user's role.");

    private static Task<Result<UserDto, ChangeUserRoleError>> HandleAsync(
        UserId userId,
        UserRoleChange change,
        [FromServices] IChangeUserRoleCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new ChangeUserRoleCommand(userId, change), cancellationToken);
}
