using System.ComponentModel;

namespace Hlibz.Stack.Application.Dto;

/// <summary>A user.</summary>
public sealed class UserDto
{
    /// <summary>Gets the user's id.</summary>
    public required UserId Id { get; init; }

    /// <summary>Gets the email address the user signs in with.</summary>
    [Sortable(IsDefault = true)]
    public required string Email { get; init; }

    /// <summary>Gets the name the user goes by.</summary>
    [Sortable]
    public required string Name { get; init; }

    /// <summary>Gets what the user may do.</summary>
    [Sortable]
    public required Role Role { get; init; }

    /// <summary>Gets when the user signed up.</summary>
    [Sortable(DefaultDirection = ListSortDirection.Descending)]
    public required DateTimeOffset CreatedAt { get; init; }

    internal static UserDto From(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role,
        CreatedAt = user.CreatedAt,
    };
}
