namespace Hlibz.Stack.Application.Dto;

/// <summary>A todo list with its items.</summary>
public sealed class TodoListDetailDto
{
    /// <summary>Gets the list's id.</summary>
    public required TodoListId Id { get; init; }

    /// <summary>Gets the user the list belongs to.</summary>
    public required UserId OwnerId { get; init; }

    /// <summary>Gets the list's title.</summary>
    public required string Title { get; init; }

    /// <summary>Gets when the list was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets when the list or its items last changed, if ever.</summary>
    public required DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>Gets the items, in their order on the list.</summary>
    public required IReadOnlyList<TodoItemDto> Items { get; init; }

    internal static TodoListDetailDto From(TodoList list) => new()
    {
        Id = list.Id,
        OwnerId = list.OwnerId,
        Title = list.Title,
        CreatedAt = list.CreatedAt,
        UpdatedAt = list.UpdatedAt,
        Items = [.. list.Items.OrderBy(item => item.Position).Select(TodoItemDto.From)],
    };
}
