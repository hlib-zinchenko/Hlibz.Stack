using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Lists the signed-in user's todo lists, newest first unless sorted otherwise.</summary>
internal sealed class GetTodoListsEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet(string.Empty, HandleAsync).WithSummary("Lists your todo lists.");

    private static Task<Result<PagedData<TodoListDto>>> HandleAsync(
        [AsParameters] TodoListsParameters parameters,
        [FromServices] IGetTodoListsQueryHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(parameters.ToQuery(ownerId: null), cancellationToken);
}
