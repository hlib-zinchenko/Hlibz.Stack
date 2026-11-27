using Hlibz.Stack.Infrastructure.Database;

namespace Hlibz.Stack.Infrastructure.Repositories;

internal sealed class TodoListRepository(StackDbContext db) : ITodoListRepository
{
    public Task<TodoList?> GetByIdAsync(TodoListId id, CancellationToken cancellationToken) =>
        db.TodoLists
            .Include(list => list.Items)
            .FirstOrDefaultAsync(list => list.Id == id, cancellationToken);

    // NOCASE: SQLite's case-insensitive comparison (ASCII letters only).
    public async Task<TodoListId?> FindIdByTitleAsync(
        UserId ownerId,
        string title,
        CancellationToken cancellationToken)
    {
        string trimmed = title.Trim();
        List<TodoListId> ids = await db.TodoLists
            .Where(list => list.OwnerId == ownerId && EF.Functions.Collate(list.Title, "NOCASE") == trimmed)
            .Select(list => list.Id)
            .Take(1)
            .ToListAsync(cancellationToken);
        return ids.Count == 0 ? null : ids[0];
    }

    public Task<int> CountByOwnerAsync(UserId ownerId, CancellationToken cancellationToken) =>
        db.TodoLists.CountAsync(list => list.OwnerId == ownerId, cancellationToken);

    public void Add(TodoList todoList) => db.TodoLists.Add(todoList);

    public void Remove(TodoList todoList) => db.TodoLists.Remove(todoList);
}
