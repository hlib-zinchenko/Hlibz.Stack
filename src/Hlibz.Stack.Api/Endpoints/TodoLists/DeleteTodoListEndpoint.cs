using Hlibz.Stack.Application.UseCases.TodoLists.DeleteTodoList;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Deletes a todo list with its items.</summary>
internal sealed class DeleteTodoListEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapDelete("{id}", HandleAsync)
            .WithSummary("Deletes a todo list.")
            .ProducesErrors(StatusCodes.Status404NotFound);

    private static Task<Result> HandleAsync(
        TodoListId id,
        [FromServices] IDeleteTodoListCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new DeleteTodoListCommand(id), cancellationToken);
}
