using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>
/// Stamps <see cref="ITimestamped"/> entities as they're saved: the creation time once, the time
/// of every later change. A change to an item stamps the list it's on too (any dependent the
/// principal holds a collection of and deletes with it), so a list's <c>updated_at</c> says when
/// anything on it changed.
/// </summary>
/// <param name="time">The clock.</param>
internal sealed class TimestampInterceptor(TimeProvider time) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            Stamp(context, time.GetUtcNow());
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void Stamp(DbContext context, DateTimeOffset now)
    {
        List<EntityEntry> changed = context.ChangeTracker.Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();
        HashSet<object> stamped = new(ReferenceEqualityComparer.Instance);
        foreach (EntityEntry entry in changed)
        {
            if (entry.Entity is ITimestamped timestamped && entry.State == EntityState.Added)
            {
                timestamped.SetCreatedAt(now);
                stamped.Add(entry.Entity);
            }
            else if (entry.Entity is ITimestamped modified && entry.State == EntityState.Modified)
            {
                modified.SetUpdatedAt(now);
                stamped.Add(entry.Entity);
            }

            foreach (EntityEntry owner in Owners(context, entry))
            {
                if (owner.Entity is ITimestamped principal
                    && owner.State is EntityState.Unchanged or EntityState.Modified
                    && stamped.Add(owner.Entity))
                {
                    principal.SetUpdatedAt(now);
                }
            }
        }
    }

    /// <summary>The tracked principals that hold <paramref name="entry"/> in a collection and delete it with them.</summary>
    private static IEnumerable<EntityEntry> Owners(DbContext context, EntityEntry entry) =>
        entry.Metadata.GetForeignKeys()
            .Where(key => key.PrincipalToDependent is { IsCollection: true }
                && key.DeleteBehavior == DeleteBehavior.Cascade)
            .SelectMany(key => context.ChangeTracker.Entries()
                .Where(candidate => candidate.Metadata == key.PrincipalEntityType
                    && SameKey(candidate, entry, key)));

    private static bool SameKey(EntityEntry principal, EntityEntry dependent, IForeignKey key) =>
        key.PrincipalKey.Properties
            .Zip(key.Properties)
            .All(pair => Equals(
                principal.Property(pair.First.Name).CurrentValue,
                dependent.Property(pair.Second.Name).OriginalValue));
}
