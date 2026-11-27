using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;

/// <summary>Adds an item to a todo list.</summary>
public interface IAddTodoItemCommandHandler : IUseCaseHandler<AddTodoItemCommand, TodoItemDto, AddTodoItemError>;
