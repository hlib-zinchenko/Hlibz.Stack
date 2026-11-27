using Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Takes an item off a todo list.</summary>
internal sealed class RemoveTodoItemEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapDelete("{id}/items/{itemId}", HandleAsync)
            .WithSummary("Removes an item.");

    private static Task<Result<Unit, RemoveTodoItemError>> HandleAsync(
        TodoListId id,
        TodoItemId itemId,
        [FromServices] IRemoveTodoItemCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new RemoveTodoItemCommand(id, itemId), cancellationToken);
}
