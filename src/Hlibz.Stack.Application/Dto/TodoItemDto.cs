namespace Hlibz.Stack.Application.Dto;

/// <summary>One thing to do, on a todo list.</summary>
public sealed class TodoItemDto
{
    /// <summary>Gets the item's id.</summary>
    public required TodoItemId Id { get; init; }

    /// <summary>Gets what's to be done.</summary>
    public required string Title { get; init; }

    /// <summary>Gets the item's place on its list, from 1.</summary>
    public required int Position { get; init; }

    /// <summary>Gets a value indicating whether the item is done.</summary>
    public required bool IsDone { get; init; }

    /// <summary>Gets when the item was done; <see langword="null"/> while it's still to do.</summary>
    public required DateTimeOffset? DoneAt { get; init; }

    internal static TodoItemDto From(TodoItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Position = item.Position,
        IsDone = item.IsDone,
        DoneAt = item.DoneAt,
    };
}
