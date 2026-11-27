using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Adds an item at the end of a todo list.</summary>
internal sealed class AddTodoItemEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPost("{id}/items", HandleAsync)
            .WithSummary("Adds an item.");

    private static Task<Result<TodoItemDto, AddTodoItemError>> HandleAsync(
        TodoListId id,
        NewTodoItem item,
        [FromServices] IAddTodoItemCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new AddTodoItemCommand(id, item), cancellationToken);
}
