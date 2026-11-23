using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

internal sealed class UpdateTodoItemCommandHandler(
    TodoListAccess access,
    IUnitOfWork unitOfWork,
    TimeProvider time) : IUpdateTodoItemCommandHandler
{
    public async Task<Result<TodoItemDto>> HandleAsync(
        UpdateTodoItemCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Result<TodoList> found = await access.GetForChangeAsync(useCase.TodoListId, cancellationToken);
        if (found is not { IsSuccess: true, Value: { } list })
        {
            return Result<TodoItemDto>.FromFailed(found);
        }

        if (list.FindItem(useCase.ItemId) is not { } item)
        {
            return Result<TodoItemDto>.NotFound(StackErrors.TodoItemNotFound);
        }

        if (useCase.Changes.Title is { } title)
        {
            item.Rename(title);
        }

        if (useCase.Changes.IsDone is { } isDone)
        {
            item.MarkDone(isDone, time.GetUtcNow());
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TodoItemDto>.Success(TodoItemDto.From(item));
    }
}
