using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

/// <summary>Lists a user's todo lists.</summary>
public interface IGetTodoListsQueryHandler
    : IUseCaseHandler<GetTodoListsQuery, PagedData<TodoListDto>>;
