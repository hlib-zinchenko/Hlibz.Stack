namespace Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;

/// <summary>What removing an item fails with.</summary>
public union RemoveTodoItemError(TodoListNotFound, TodoItemNotFound);
