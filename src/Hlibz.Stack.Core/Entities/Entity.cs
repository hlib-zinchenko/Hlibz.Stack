namespace Hlibz.Stack.Core.Entities;

/// <summary>An entity: an id, and the times it was created and last changed.</summary>
/// <typeparam name="TId">The entity's strongly-typed id.</typeparam>
public abstract class Entity<TId> : ITimestamped
    where TId : struct, IEntityId<TId>
{
    /// <summary>Initializes a new instance of the <see cref="Entity{TId}"/> class.</summary>
    /// <param name="id">The entity's id.</param>
    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>Gets the entity's id.</summary>
    public TId Id { get; private set; }

    /// <summary>Gets when the entity was first saved.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Gets when the entity was last saved with changes, if ever.</summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    [SuppressMessage(
        "Design",
        "CA1033:Interface methods should be callable by child types",
        Justification = "Only the save interceptor, through ITimestamped, sets this.")]
    void ITimestamped.SetCreatedAt(DateTimeOffset createdAt) => CreatedAt = createdAt;

    [SuppressMessage(
        "Design",
        "CA1033:Interface methods should be callable by child types",
        Justification = "Only the save interceptor, through ITimestamped, sets this.")]
    void ITimestamped.SetUpdatedAt(DateTimeOffset updatedAt) => UpdatedAt = updatedAt;
}
