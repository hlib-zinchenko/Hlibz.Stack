namespace Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;

/// <summary>Takes an item off a todo list.</summary>
/// <param name="TodoListId">The list's id.</param>
/// <param name="ItemId">The item's id.</param>
public sealed record RemoveTodoItemCommand(TodoListId TodoListId, TodoItemId ItemId) : IUseCase;
