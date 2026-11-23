namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;

/// <summary>Gets a todo list with its items: the signed-in user's own, or anyone's for an admin.</summary>
/// <param name="Id">The list's id.</param>
public sealed record GetTodoListQuery(TodoListId Id) : IUseCase;
