namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

/// <summary>The todo list holds as many items as it can: remove one first.</summary>
/// <param name="MaxItems">How many items a list holds.</param>
[Error("stack_1009", ResultStatus.Conflict, "The todo list holds as many items as it can.")]
public sealed record TodoListFull(int MaxItems) : Error;
