namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

/// <summary>A todo list's new title.</summary>
/// <param name="Title">The title.</param>
public sealed record TodoListChanges([Required, MaxLength(TodoList.TitleMaxLength)] string Title);
