using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.ReadRepositories;

namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

internal sealed class GetTodoListsQueryHandler(ICallerResolver callers, ITodoListReadRepository lists)
    : IGetTodoListsQueryHandler
{
    public async Task<Result<PagedData<TodoListDto>>> HandleAsync(
        GetTodoListsQuery useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        UserId ownerId = useCase.OwnerId ?? caller.Id;
        if (!caller.CanManage(ownerId))
        {
            return Result<PagedData<TodoListDto>>.Forbidden();
        }

        return Result<PagedData<TodoListDto>>.Success(await lists.GetPageAsync(
            ownerId,
            useCase.Search,
            SortableDto.For<TodoListDto>().Parse(useCase.Sort),
            new PagingParams(useCase.Page, useCase.PerPage),
            cancellationToken));
    }
}
