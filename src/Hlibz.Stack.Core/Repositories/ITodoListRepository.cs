using Hlibz.Stack.Core.TodoLists;

namespace Hlibz.Stack.Core.Repositories;

/// <summary>Loads todo lists, with their items, to change them; stages new and deleted ones.</summary>
public interface ITodoListRepository
{
    /// <summary>Loads a list with its items.</summary>
    /// <param name="id">The list's id.</param>
    /// <param name="cancellationToken">Cancels the load.</param>
    /// <returns>The list; <see langword="null"/> when there's none with that id.</returns>
    Task<TodoList?> GetByIdAsync(TodoListId id, CancellationToken cancellationToken);

    /// <summary>Stages a new list, saved with the unit of work.</summary>
    /// <param name="todoList">The list.</param>
    void Add(TodoList todoList);

    /// <summary>Stages a list's deletion, with its items.</summary>
    /// <param name="todoList">The list.</param>
    void Remove(TodoList todoList);
}
