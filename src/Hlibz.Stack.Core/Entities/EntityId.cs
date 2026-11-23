namespace Hlibz.Stack.Core.Entities;

/// <summary>Makes strongly-typed ids.</summary>
public static class EntityId
{
    /// <summary>Makes a new, time-ordered id (a version 7 <see cref="Guid"/>).</summary>
    /// <typeparam name="TId">The id type.</typeparam>
    /// <returns>The id.</returns>
    public static TId New<TId>()
        where TId : struct, IEntityId<TId> =>
        TId.From(Guid.CreateVersion7());
}
