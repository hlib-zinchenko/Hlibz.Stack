using Hlibz.Stack.Core.Entities;

namespace Hlibz.Stack.Core.TodoLists;

/// <summary>The id of an item on a todo list.</summary>
[StronglyTypedId(Template.Guid)]
public readonly partial struct TodoItemId : IEntityId<TodoItemId>
{
    /// <inheritdoc/>
    public static TodoItemId From(Guid value) => new(value);
}
