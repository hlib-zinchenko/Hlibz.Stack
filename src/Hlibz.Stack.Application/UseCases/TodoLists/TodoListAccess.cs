namespace Hlibz.Stack.Application.UseCases.TodoLists;

/// <summary>
/// Loads a todo list for the caller to change: their own, or anyone's for an admin. Someone
/// else's list is not found, so its id says nothing about whether it exists.
/// </summary>
/// <param name="callers">Finds the caller.</param>
/// <param name="lists">Loads the list.</param>
internal sealed class TodoListAccess(ICallerResolver callers, ITodoListRepository lists)
{
    public async Task<Result<TodoList>> GetForChangeAsync(
        TodoListId id,
        CancellationToken cancellationToken)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        TodoList? list = await lists.GetByIdAsync(id, cancellationToken);
        return list is not null && caller.CanManage(list.OwnerId)
            ? Result<TodoList>.Success(list)
            : Result<TodoList>.NotFound(StackErrors.TodoListNotFound);
    }
}
