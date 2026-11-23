using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.ReadRepositories;

namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;

internal sealed class GetTodoListQueryHandler(ICallerResolver callers, ITodoListReadRepository lists)
    : IGetTodoListQueryHandler
{
    public async Task<Result<TodoListDetailDto>> HandleAsync(
        GetTodoListQuery useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        TodoListDetailDto? list = await lists.GetAsync(useCase.Id, cancellationToken);
        return list is not null && caller.CanManage(list.OwnerId)
            ? Result<TodoListDetailDto>.Success(list)
            : Result<TodoListDetailDto>.NotFound(StackErrors.TodoListNotFound);
    }
}
