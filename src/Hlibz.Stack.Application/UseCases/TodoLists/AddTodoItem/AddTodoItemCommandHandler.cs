using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

internal sealed class AddTodoItemCommandHandler(TodoListAccess access, IUnitOfWork unitOfWork)
    : IAddTodoItemCommandHandler
{
    public async Task<Result<TodoItemDto>> HandleAsync(
        AddTodoItemCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Result<TodoList> found = await access.GetForChangeAsync(useCase.TodoListId, cancellationToken);
        if (found is not { IsSuccess: true, Value: { } list })
        {
            return Result<TodoItemDto>.FromFailed(found);
        }

        TodoItem item = list.AddItem(useCase.Item.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<TodoItemDto>.Success(TodoItemDto.From(item));
    }
}
