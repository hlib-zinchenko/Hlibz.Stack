namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

/// <summary>Adds an item at the end of a todo list.</summary>
/// <param name="TodoListId">The list's id.</param>
/// <param name="Item">The item.</param>
public sealed record AddTodoItemCommand(TodoListId TodoListId, NewTodoItem Item) : IUseCase;
