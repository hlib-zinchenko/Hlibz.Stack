using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;

internal sealed class CreateTodoListCommandHandler(
    ICallerResolver callers,
    ITodoListRepository lists,
    IUnitOfWork unitOfWork) : ICreateTodoListCommandHandler
{
    public async Task<Result<TodoListDetailDto>> HandleAsync(
        CreateTodoListCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        TodoList list = TodoList.Create(caller.Id, useCase.Title);
        lists.Add(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TodoListDetailDto>.Success(TodoListDetailDto.From(list));
    }
}
