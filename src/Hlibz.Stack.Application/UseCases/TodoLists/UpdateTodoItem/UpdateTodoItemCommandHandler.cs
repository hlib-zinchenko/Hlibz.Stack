using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

internal sealed class UpdateTodoItemCommandHandler(
    TodoListAccess access,
    IUnitOfWork unitOfWork,
    TimeProvider time) : IUpdateTodoItemCommandHandler
{
    public async Task<Result<TodoItemDto, UpdateTodoItemError>> HandleAsync(
        UpdateTodoItemCommand useCase,
        CancellationToken cancellationToken = default)
    {
        if (await access.FindForChangeAsync(useCase.TodoListId, cancellationToken) is not { } list)
        {
            return new TodoListNotFound(useCase.TodoListId);
        }

        if (list.FindItem(useCase.ItemId) is not { } item)
        {
            return new TodoItemNotFound(useCase.ItemId);
        }

        if (useCase.Changes.Title is { } title)
        {
            if (list.FindItemByTitle(title) is { } namesake && namesake != item)
            {
                return new TodoItemTitleTaken(namesake.Id);
            }

            item.Rename(title);
        }

        if (useCase.Changes.IsDone is { } isDone)
        {
            item.MarkDone(isDone, time.GetUtcNow());
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TodoItemDto.From(item);
    }
}
