using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.ReadRepositories;

/// <summary>Reads users for lists, never as tracked entities.</summary>
internal interface IUserReadRepository
{
    /// <summary>Gets one page of users.</summary>
    /// <param name="search">Text the email address or name contains; <see langword="null"/> for all.</param>
    /// <param name="sorting">The order.</param>
    /// <param name="paging">The page.</param>
    /// <param name="cancellationToken">Cancels the read.</param>
    /// <returns>The page of users.</returns>
    Task<PagedData<UserDto>> GetPageAsync(
        string? search,
        SortingParams sorting,
        PagingParams paging,
        CancellationToken cancellationToken);
}
