namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

/// <summary>What changing an item fails with.</summary>
public union UpdateTodoItemError(TodoListNotFound, TodoItemNotFound, TodoItemTitleTaken);
