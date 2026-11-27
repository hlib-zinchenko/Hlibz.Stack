namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

/// <summary>What creating a todo list fails with.</summary>
public union CreateTodoListError(TodoListTitleTaken, TodoListLimitReached);
