namespace Hlibz.Stack.Core.Entities;

/// <summary>A strongly-typed entity id, stored as the <see cref="Guid"/> it wraps.</summary>
public interface IEntityId
{
    /// <summary>Gets the wrapped value.</summary>
    Guid Value { get; }
}

/// <summary>A strongly-typed entity id that can be made from its value, as EF Core does.</summary>
/// <typeparam name="TSelf">The id type itself.</typeparam>
public interface IEntityId<TSelf> : IEntityId
    where TSelf : struct, IEntityId<TSelf>
{
    /// <summary>Wraps <paramref name="value"/>.</summary>
    /// <param name="value">The value.</param>
    /// <returns>The id.</returns>
    static abstract TSelf From(Guid value);
}
