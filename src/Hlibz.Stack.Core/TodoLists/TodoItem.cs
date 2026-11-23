using Hlibz.Stack.Core.Entities;

namespace Hlibz.Stack.Core.TodoLists;

/// <summary>One thing to do, on a todo list.</summary>
public sealed class TodoItem : Entity<TodoItemId>
{
    /// <summary>The longest title.</summary>
    public const int TitleMaxLength = 200;

    private TodoItem(TodoItemId id, TodoListId todoListId, string title, int position)
        : base(id)
    {
        TodoListId = todoListId;
        Title = title;
        Position = position;
    }

    /// <summary>Gets the list the item is on.</summary>
    public TodoListId TodoListId { get; private set; }

    /// <summary>Gets what's to be done.</summary>
    public string Title { get; private set; }

    /// <summary>Gets the item's place on its list, from 1.</summary>
    public int Position { get; private set; }

    /// <summary>Gets when the item was done; <see langword="null"/> while it's still to do.</summary>
    public DateTimeOffset? DoneAt { get; private set; }

    /// <summary>Gets a value indicating whether the item is done.</summary>
    public bool IsDone => DoneAt is not null;

    /// <summary>Changes what's to be done.</summary>
    /// <param name="title">The new title, trimmed here.</param>
    public void Rename(string title) => Title = title.Trim();

    /// <summary>Marks the item done, or to do again.</summary>
    /// <param name="isDone">Whether it's done.</param>
    /// <param name="now">The current time, kept as when it was done.</param>
    public void MarkDone(bool isDone, DateTimeOffset now)
    {
        if (isDone != IsDone)
        {
            DoneAt = isDone ? now : null;
        }
    }

    /// <summary>Creates an item.</summary>
    /// <param name="todoListId">The list it goes on.</param>
    /// <param name="title">What's to be done, trimmed here.</param>
    /// <param name="position">Its place on the list.</param>
    /// <returns>The item.</returns>
    internal static TodoItem Create(TodoListId todoListId, string title, int position) =>
        new(EntityId.New<TodoItemId>(), todoListId, title.Trim(), position);
}
