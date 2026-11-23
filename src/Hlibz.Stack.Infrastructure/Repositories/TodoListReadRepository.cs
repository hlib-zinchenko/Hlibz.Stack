using Hlibz.Paging.EntityFrameworkCore;
using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.ReadRepositories;
using Hlibz.Stack.Infrastructure.Database;

namespace Hlibz.Stack.Infrastructure.Repositories;

internal sealed class TodoListReadRepository(StackDbContext db) : ITodoListReadRepository
{
    public Task<PagedData<TodoListDto>> GetPageAsync(
        UserId ownerId,
        string? search,
        SortingParams sorting,
        PagingParams paging,
        CancellationToken cancellationToken)
    {
        IQueryable<TodoList> lists = db.TodoLists.AsNoTracking().Where(list => list.OwnerId == ownerId);
        if (!string.IsNullOrWhiteSpace(search))
        {
            string pattern = $"%{search.Trim()}%";
            lists = lists.Where(list => EF.Functions.Like(list.Title, pattern));
        }

        return lists
            .Select(list => new TodoListDto
            {
                Id = list.Id,
                OwnerId = list.OwnerId,
                Title = list.Title,
                ItemCount = list.Items.Count,
                DoneCount = list.Items.Count(item => item.DoneAt != null),
                CreatedAt = list.CreatedAt,
                UpdatedAt = list.UpdatedAt,
            })
            .PaginateAsync(sorting, paging, cancellationToken);
    }

    public async Task<TodoListDetailDto?> GetAsync(TodoListId id, CancellationToken cancellationToken)
    {
        TodoList? list = await db.TodoLists
            .AsNoTracking()
            .Include(list => list.Items)
            .FirstOrDefaultAsync(list => list.Id == id, cancellationToken);
        return list is null ? null : TodoListDetailDto.From(list);
    }
}
