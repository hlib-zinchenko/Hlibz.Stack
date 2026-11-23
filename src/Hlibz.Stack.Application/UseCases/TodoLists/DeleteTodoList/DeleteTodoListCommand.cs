namespace Hlibz.Stack.Application.UseCases.TodoLists.DeleteTodoList;

/// <summary>Deletes a todo list with its items.</summary>
/// <param name="Id">The list's id.</param>
public sealed record DeleteTodoListCommand(TodoListId Id) : IUseCase;
