using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

internal sealed class AddTodoItemCommandHandler(TodoListAccess access, IUnitOfWork unitOfWork)
    : IAddTodoItemCommandHandler
{
    public async Task<Result<TodoItemDto, AddTodoItemError>> HandleAsync(
        AddTodoItemCommand useCase,
        CancellationToken cancellationToken = default)
    {
        if (await access.FindForChangeAsync(useCase.TodoListId, cancellationToken) is not { } list)
        {
            return new TodoListNotFound(useCase.TodoListId);
        }

        if (list.FindItemByTitle(useCase.Item.Title) is { } namesake)
        {
            return new TodoItemTitleTaken(namesake.Id);
        }

        if (list.Items.Count >= TodoList.MaxItems)
        {
            return new TodoListFull(TodoList.MaxItems);
        }

        TodoItem item = list.AddItem(useCase.Item.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return TodoItemDto.From(item);
    }
}
