using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Gets a todo list with its items.</summary>
internal sealed class GetTodoListEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapGet("{id}", HandleAsync)
            .WithSummary("Gets a todo list.");

    private static Task<Result<TodoListDetailDto, GetTodoListError>> HandleAsync(
        TodoListId id,
        [FromServices] IGetTodoListQueryHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new GetTodoListQuery(id), cancellationToken);
}
