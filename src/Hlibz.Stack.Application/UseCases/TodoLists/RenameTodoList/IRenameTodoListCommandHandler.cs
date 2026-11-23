using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;

/// <summary>Renames a todo list.</summary>
public interface IRenameTodoListCommandHandler
    : IUseCaseHandler<RenameTodoListCommand, TodoListDetailDto>;
