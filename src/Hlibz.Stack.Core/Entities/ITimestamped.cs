namespace Hlibz.Stack.Core.Entities;

/// <summary>An entity whose creation and last change the save interceptor stamps.</summary>
public interface ITimestamped
{
    /// <summary>Stamps the creation time.</summary>
    /// <param name="createdAt">When the entity was first saved.</param>
    void SetCreatedAt(DateTimeOffset createdAt);

    /// <summary>Stamps the time of the last change.</summary>
    /// <param name="updatedAt">When the entity was last saved with changes.</param>
    void SetUpdatedAt(DateTimeOffset updatedAt);
}
