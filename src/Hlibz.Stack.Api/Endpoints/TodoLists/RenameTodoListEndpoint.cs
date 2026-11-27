using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>Renames a todo list.</summary>
internal sealed class RenameTodoListEndpoint : IEndpoint<TodoListsEndpointGroup>
{
    public RouteHandlerBuilder Map(IEndpointRouteBuilder group) =>
        group.MapPut("{id}", HandleAsync)
            .WithSummary("Renames a todo list.");

    private static Task<Result<TodoListDetailDto, RenameTodoListError>> HandleAsync(
        TodoListId id,
        TodoListChanges changes,
        [FromServices] IRenameTodoListCommandHandler handler,
        CancellationToken cancellationToken) =>
        handler.HandleAsync(new RenameTodoListCommand(id, changes), cancellationToken);
}
