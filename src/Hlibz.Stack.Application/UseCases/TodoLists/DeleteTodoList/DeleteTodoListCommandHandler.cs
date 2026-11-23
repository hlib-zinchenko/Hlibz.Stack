namespace Hlibz.Stack.Application.UseCases.TodoLists.DeleteTodoList;

internal sealed class DeleteTodoListCommandHandler(
    TodoListAccess access,
    ITodoListRepository lists,
    IUnitOfWork unitOfWork) : IDeleteTodoListCommandHandler
{
    public async Task<Result> HandleAsync(
        DeleteTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Result<TodoList> found = await access.GetForChangeAsync(useCase.Id, cancellationToken);
        if (found is not { IsSuccess: true, Value: { } list })
        {
            return found;
        }

        lists.Remove(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
