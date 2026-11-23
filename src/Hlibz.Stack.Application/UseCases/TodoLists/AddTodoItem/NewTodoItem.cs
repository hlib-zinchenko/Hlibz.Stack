namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

/// <summary>An item to add to a todo list.</summary>
/// <param name="Title">What's to be done.</param>
public sealed record NewTodoItem([Required, MaxLength(TodoItem.TitleMaxLength)] string Title);
