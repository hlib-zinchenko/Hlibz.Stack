using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

/// <summary>Creates a todo list.</summary>
public interface ICreateTodoListCommandHandler
    : IUseCaseHandler<CreateTodoListCommand, TodoListDetailDto>;
