using System.Collections.Concurrent;

using Hlibz.Stack.Core.TodoLists;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Hlibz.Stack.Tests;

/// <summary>
/// Deletes an armed todo list's row just before a save that changes the list, as if someone else
/// deleted it meanwhile: the save's update then matches no row, which EF Core reports as a
/// concurrency conflict.
/// </summary>
public sealed class ConcurrentDelete : SaveChangesInterceptor
{
    private readonly ConcurrentDictionary<Guid, bool> _armed = [];

    public void Arm(string todoListId) => _armed[Guid.Parse(todoListId)] = true;

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            EntityEntry<TodoList>[] changed =
                [.. context.ChangeTracker.Entries<TodoList>().Where(entry => entry.State == EntityState.Modified)];
            foreach (EntityEntry<TodoList> entry in changed)
            {
                TodoListId id = entry.Entity.Id;
                if (_armed.TryRemove(id.Value, out _))
                {
                    await context.Set<TodoList>().Where(list => list.Id == id).ExecuteDeleteAsync(cancellationToken);
                }
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
