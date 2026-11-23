using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.ReadRepositories;

/// <summary>Reads todo lists for display, never as tracked entities.</summary>
internal interface ITodoListReadRepository
{
    /// <summary>Gets one page of a user's lists.</summary>
    /// <param name="ownerId">The user.</param>
    /// <param name="search">Text the title contains; <see langword="null"/> for all.</param>
    /// <param name="sorting">The order.</param>
    /// <param name="paging">The page.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The page of lists.</returns>
    Task<PagedData<TodoListDto>> GetPageAsync(
        UserId ownerId,
        string? search,
        SortingParams sorting,
        PagingParams paging,
        CancellationToken cancellationToken);

    /// <summary>Gets a list with its items.</summary>
    /// <param name="id">The list's id.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The list; <see langword="null"/> when there's none with that id.</returns>
    Task<TodoListDetailDto?> GetAsync(TodoListId id, CancellationToken cancellationToken);
}
