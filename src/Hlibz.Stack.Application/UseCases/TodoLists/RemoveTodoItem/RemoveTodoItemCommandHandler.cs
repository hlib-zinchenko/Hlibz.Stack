namespace Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;

internal sealed class RemoveTodoItemCommandHandler(TodoListAccess access, IUnitOfWork unitOfWork)
    : IRemoveTodoItemCommandHandler
{
    public async Task<Result<Unit, RemoveTodoItemError>> HandleAsync(
        RemoveTodoItemCommand useCase,
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

        list.RemoveItem(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
