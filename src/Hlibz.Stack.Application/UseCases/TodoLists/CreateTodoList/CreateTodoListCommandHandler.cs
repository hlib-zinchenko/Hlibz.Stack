using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

internal sealed class CreateTodoListCommandHandler(
    ICallerResolver callers,
    ITodoListRepository lists,
    IUnitOfWork unitOfWork) : ICreateTodoListCommandHandler
{
    public async Task<Result<TodoListDetailDto, CreateTodoListError>> HandleAsync(
        CreateTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        if (await lists.FindIdByTitleAsync(caller.Id, useCase.Title, cancellationToken) is { } takenBy)
        {
            return new TodoListTitleTaken(takenBy);
        }

        if (await lists.CountByOwnerAsync(caller.Id, cancellationToken) >= TodoList.MaxPerOwner)
        {
            return new TodoListLimitReached(TodoList.MaxPerOwner);
        }

        TodoList list = TodoList.Create(caller.Id, useCase.Title);
        lists.Add(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TodoListDetailDto.From(list);
    }
}
