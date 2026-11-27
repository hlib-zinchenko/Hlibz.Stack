using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

/// <summary>Changes an item on a todo list.</summary>
public interface IUpdateTodoItemCommandHandler : IUseCaseHandler<UpdateTodoItemCommand, TodoItemDto, UpdateTodoItemError>;
