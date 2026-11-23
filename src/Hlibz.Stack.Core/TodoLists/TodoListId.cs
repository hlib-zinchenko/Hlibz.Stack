using Hlibz.Stack.Core.Entities;

namespace Hlibz.Stack.Core.TodoLists;

/// <summary>The id of a todo list.</summary>
[StronglyTypedId(Template.Guid)]
public readonly partial struct TodoListId : IEntityId<TodoListId>
{
    /// <inheritdoc/>
    public static TodoListId From(Guid value) => new(value);
}
