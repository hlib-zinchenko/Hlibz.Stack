using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Renames an item, marks it done or to do again, or both.</summary>
internal sealed class UpdateTodoItemEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPatch("{id}/items/{itemId}", HandleAsync)
            .WithSummary("Changes an item.")
            .ProducesErrors(StatusCodes.Status404NotFound);

    private static Task<Result<TodoItemDto>> HandleAsync(
        TodoListId id,
        TodoItemId itemId,
        TodoItemChanges changes,
        [FromServices] IUpdateTodoItemCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new UpdateTodoItemCommand(id, itemId, changes), cancellationToken);
}
