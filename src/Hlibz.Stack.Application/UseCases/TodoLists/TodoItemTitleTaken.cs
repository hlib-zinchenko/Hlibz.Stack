namespace Hlibz.Stack.Application.UseCases.TodoLists;

/// <summary>The todo list already has an item with the title.</summary>
/// <param name="ItemId">The id of the item that has it.</param>
[Error("stack_1010", ResultStatus.Conflict, "The todo list already has an item with this title.")]
public sealed record TodoItemTitleTaken(TodoItemId ItemId) : Error;
