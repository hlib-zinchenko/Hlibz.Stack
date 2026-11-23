using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>Stores a strongly-typed id as the <see cref="Guid"/> it wraps.</summary>
/// <typeparam name="TId">The id type.</typeparam>
internal sealed class EntityIdConverter<TId>() : ValueConverter<TId, Guid>(
    id => id.Value,
    value => Create(value))
    where TId : struct, IEntityId<TId>
{
    // An expression tree can't call a static abstract member, so it calls this instead.
    private static TId Create(Guid value) => TId.From(value);
}
