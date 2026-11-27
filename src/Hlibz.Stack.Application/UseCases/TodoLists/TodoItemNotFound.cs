namespace Hlibz.Stack.Application.UseCases.TodoLists;

/// <summary>The todo list has no item with the id.</summary>
/// <param name="ItemId">The item's id.</param>
[Error("stack_1004", ResultStatus.NotFound, "The todo list has no item with this id.")]
public sealed record TodoItemNotFound(TodoItemId ItemId) : Error;
