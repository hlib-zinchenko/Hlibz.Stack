namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

/// <summary>The user has as many todo lists as they may: delete one first.</summary>
/// <param name="MaxTodoLists">How many todo lists a user may have.</param>
[Error("stack_1008", ResultStatus.Conflict, "The user has as many todo lists as they may.")]
public sealed record TodoListLimitReached(int MaxTodoLists) : Error;
