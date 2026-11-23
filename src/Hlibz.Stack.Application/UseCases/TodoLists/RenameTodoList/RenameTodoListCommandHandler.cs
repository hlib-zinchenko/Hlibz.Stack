using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

internal sealed class RenameTodoListCommandHandler(TodoListAccess access, IUnitOfWork unitOfWork)
    : IRenameTodoListCommandHandler
{
    public async Task<Result<TodoListDetailDto>> HandleAsync(
        RenameTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Result<TodoList> found = await access.GetForChangeAsync(useCase.Id, cancellationToken);
        if (found is not { IsSuccess: true, Value: { } list })
        {
            return Result<TodoListDetailDto>.FromFailed(found);
        }

        list.Rename(useCase.Changes.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TodoListDetailDto>.Success(TodoListDetailDto.From(list));
    }
}
