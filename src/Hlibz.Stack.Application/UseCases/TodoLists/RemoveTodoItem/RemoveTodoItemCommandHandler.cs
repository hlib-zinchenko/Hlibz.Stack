namespace Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;

internal sealed class RemoveTodoItemCommandHandler(TodoListAccess access, IUnitOfWork unitOfWork)
    : IRemoveTodoItemCommandHandler
{
    public async Task<Result> HandleAsync(
        RemoveTodoItemCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Result<TodoList> found = await access.GetForChangeAsync(useCase.TodoListId, cancellationToken);
        if (found is not { IsSuccess: true, Value: { } list })
        {
            return found;
        }

        if (list.FindItem(useCase.ItemId) is not { } item)
        {
            return Result.NotFound(StackErrors.TodoItemNotFound);
        }

        list.RemoveItem(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
