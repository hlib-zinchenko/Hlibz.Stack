namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

/// <summary>Renames a todo list.</summary>
/// <param name="Id">The list's id.</param>
/// <param name="Changes">The new title.</param>
public sealed record RenameTodoListCommand(TodoListId Id, TodoListChanges Changes) : IUseCase;
