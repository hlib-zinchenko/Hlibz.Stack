using Hlibz.Paging.EntityFrameworkCore;
using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.ReadRepositories;
using Hlibz.Stack.Infrastructure.Database;

namespace Hlibz.Stack.Infrastructure.Repositories;

internal sealed class UserReadRepository(StackDbContext db) : IUserReadRepository
{
    public Task<PagedData<UserDto>> GetPageAsync(
        string? search,
        SortingParams sorting,
        PagingParams paging,
        CancellationToken cancellationToken)
    {
        IQueryable<User> users = db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search.Trim()}%";
            users = users.Where(user =>
                EF.Functions.Like(user.Email, pattern) || EF.Functions.Like(user.Name, pattern));
        }

        return users
            .Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
            })
            .PaginateAsync(sorting, paging, cancellationToken);
    }
}
