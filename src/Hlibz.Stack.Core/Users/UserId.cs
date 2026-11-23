using Hlibz.Stack.Core.Entities;

namespace Hlibz.Stack.Core.Users;

/// <summary>The id of a user.</summary>
[StronglyTypedId(Template.Guid)]
public readonly partial struct UserId : IEntityId<UserId>
{
    /// <inheritdoc/>
    public static UserId From(Guid value) => new(value);
}
