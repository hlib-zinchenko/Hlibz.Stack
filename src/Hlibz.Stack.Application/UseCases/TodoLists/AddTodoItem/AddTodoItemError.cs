namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

/// <summary>What adding an item fails with.</summary>
public union AddTodoItemError(TodoListNotFound, TodoItemTitleTaken, TodoListFull);
