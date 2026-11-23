using Hlibz.Stack.Infrastructure.Database;

namespace Hlibz.Stack.Infrastructure.Repositories;

internal sealed class TodoListRepository(StackDbContext db) : ITodoListRepository
{
    public Task<TodoList?> GetByIdAsync(TodoListId id, CancellationToken cancellationToken) =>
        db.TodoLists
            .Include(list => list.Items)
            .FirstOrDefaultAsync(list => list.Id == id, cancellationToken);

    public void Add(TodoList todoList) => db.TodoLists.Add(todoList);

    public void Remove(TodoList todoList) => db.TodoLists.Remove(todoList);
}
