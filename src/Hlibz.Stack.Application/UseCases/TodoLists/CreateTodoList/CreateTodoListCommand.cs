namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

/// <summary>Creates an empty todo list for the signed-in user.</summary>
/// <param name="Title">The list's title.</param>
public sealed record CreateTodoListCommand(
    [Required, MaxLength(TodoList.TitleMaxLength)] string Title) : IUseCase;
