namespace Hlibz.Stack.Application.UseCases.TodoLists.DeleteTodoList;

internal sealed class DeleteTodoListCommandHandler(
    TodoListAccess access,
    ITodoListRepository lists,
    IUnitOfWork unitOfWork) : IDeleteTodoListCommandHandler
{
    public async Task<Result<Unit, DeleteTodoListError>> HandleAsync(
        DeleteTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        if (await access.FindForChangeAsync(useCase.Id, cancellationToken) is not { } list)
        {
            return new TodoListNotFound(useCase.Id);
        }

        lists.Remove(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
