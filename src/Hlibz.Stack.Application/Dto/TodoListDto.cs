using System.ComponentModel;

namespace Hlibz.Stack.Application.Dto;

/// <summary>A todo list in a list of them: its counts, not its items.</summary>
public sealed class TodoListDto
{
    /// <summary>Gets the list's id.</summary>
    public required TodoListId Id { get; init; }

    /// <summary>Gets the user the list belongs to.</summary>
    public required UserId OwnerId { get; init; }

    /// <summary>Gets the list's title.</summary>
    [Sortable]
    public required string Title { get; init; }

    /// <summary>Gets how many items the list has.</summary>
    [Sortable]
    public required int ItemCount { get; init; }

    /// <summary>Gets how many of them are done.</summary>
    public required int DoneCount { get; init; }

    /// <summary>Gets when the list was created.</summary>
    [Sortable(IsDefault = true, DefaultDirection = ListSortDirection.Descending)]
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets when the list or its items last changed, if ever.</summary>
    public required DateTimeOffset? UpdatedAt { get; init; }
}
