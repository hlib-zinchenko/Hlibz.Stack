namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

/// <summary>What renaming a todo list fails with.</summary>
public union RenameTodoListError(TodoListNotFound, TodoListTitleTaken);
