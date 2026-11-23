using Hlibz.Stack.Api.Endpoints.TodoLists;
using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

namespace Hlibz.Stack.Api.Endpoints.Admin;

/// <summary>
/// Lists a user's todo lists. Open, rename or delete one through <c>api/todo-lists/{id}</c>, as
/// its owner would.
/// </summary>
internal sealed class GetUserTodoListsEndpoint : IEndpoint<AdminUsersEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet("{userId}/todo-lists", HandleAsync).WithSummary("Lists a user's todo lists.");

    private static Task<Result<PagedData<TodoListDto>>> HandleAsync(
        UserId userId,
        [AsParameters] TodoListsParameters parameters,
        [FromServices] IGetTodoListsQueryHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(parameters.ToQuery(userId), cancellationToken);
}
