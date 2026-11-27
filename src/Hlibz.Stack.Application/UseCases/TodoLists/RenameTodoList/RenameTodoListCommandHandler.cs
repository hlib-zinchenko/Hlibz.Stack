using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

internal sealed class RenameTodoListCommandHandler(
    TodoListAccess access,
    ITodoListRepository lists,
    IUnitOfWork unitOfWork)
    : IRenameTodoListCommandHandler
{
    public async Task<Result<TodoListDetailDto, RenameTodoListError>> HandleAsync(
        RenameTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        if (await access.FindForChangeAsync(useCase.Id, cancellationToken) is not { } list)
        {
            return new TodoListNotFound(useCase.Id);
        }

        // The owner's other lists: an admin renaming someone's list keeps the owner's titles apart.
        if (await lists.FindIdByTitleAsync(list.OwnerId, useCase.Changes.Title, cancellationToken) is { } takenBy
            && takenBy != list.Id)
        {
            return new TodoListTitleTaken(takenBy);
        }

        list.Rename(useCase.Changes.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TodoListDetailDto.From(list);
    }
}
