namespace Hlibz.Stack.Application.UseCases.TodoLists;

/// <summary>The list's owner already has a todo list with the title.</summary>
/// <param name="TodoListId">The id of the list that has it.</param>
[Error("stack_1007", ResultStatus.Conflict, "The owner already has a todo list with this title.")]
public sealed record TodoListTitleTaken(TodoListId TodoListId) : Error;
