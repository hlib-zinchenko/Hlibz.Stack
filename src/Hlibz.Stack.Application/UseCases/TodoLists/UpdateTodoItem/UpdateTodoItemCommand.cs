namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

/// <summary>Renames an item on a todo list, marks it done or to do again, or both.</summary>
/// <param name="TodoListId">The list's id.</param>
/// <param name="ItemId">The item's id.</param>
/// <param name="Changes">What changes.</param>
public sealed record UpdateTodoItemCommand(
    TodoListId TodoListId,
    TodoItemId ItemId,
    TodoItemChanges Changes) : IUseCase;
