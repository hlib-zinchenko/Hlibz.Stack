using Hlibz.Stack.Core.TodoLists;
using Hlibz.Stack.Core.Users;

namespace Hlibz.Stack.Core.Repositories;

/// <summary>Loads todo lists, with their items, to change them; stages new and deleted ones.</summary>
public interface ITodoListRepository
{
    /// <summary>Loads a list with its items.</summary>
    /// <param name="id">The list's id.</param>
    /// <param name="cancellationToken">Cancels the load.</param>
    /// <returns>The list; <see langword="null"/> when there's none with that id.</returns>
    Task<TodoList?> GetByIdAsync(TodoListId id, CancellationToken cancellationToken);

    /// <summary>Finds a user's list by its title, ignoring case and surrounding spaces.</summary>
    /// <param name="ownerId">The user.</param>
    /// <param name="title">The title.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <returns>The list's id; <see langword="null"/> when the user has no list with that title.</returns>
    Task<TodoListId?> FindIdByTitleAsync(UserId ownerId, string title, CancellationToken cancellationToken);

    /// <summary>Counts a user's lists.</summary>
    /// <param name="ownerId">The user.</param>
    /// <param name="cancellationToken">Cancels the count.</param>
    /// <returns>How many lists the user has.</returns>
    Task<int> CountByOwnerAsync(UserId ownerId, CancellationToken cancellationToken);

    /// <summary>Stages a new list, saved with the unit of work.</summary>
    /// <param name="todoList">The list.</param>
    void Add(TodoList todoList);

    /// <summary>Stages a list's deletion, with its items.</summary>
    /// <param name="todoList">The list.</param>
    void Remove(TodoList todoList);
}
