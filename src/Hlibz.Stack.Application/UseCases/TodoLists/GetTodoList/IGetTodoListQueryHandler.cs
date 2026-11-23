using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;

/// <summary>Gets a todo list.</summary>
public interface IGetTodoListQueryHandler : IUseCaseHandler<GetTodoListQuery, TodoListDetailDto>;
