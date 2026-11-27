namespace Hlibz.Stack.Application.UseCases.TodoLists;

/// <summary>No todo list with the id is visible to the caller: someone else's list is not found either.</summary>
/// <param name="TodoListId">The list's id.</param>
[Error("stack_1003", ResultStatus.NotFound, "There's no todo list with this id.")]
public sealed record TodoListNotFound(TodoListId TodoListId) : Error;
