using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.GetUsers;

/// <summary>Lists every user, one page at a time.</summary>
/// <param name="Search">Text the email address or name contains.</param>
/// <param name="Page">The page, from 1.</param>
/// <param name="PerPage">Users per page.</param>
/// <param name="Sort">The order.</param>
[AdminOnly]
public sealed record GetUsersQuery(
    string? Search,
    [Range(1, int.MaxValue)] int? Page,
    [Range(1, 100)] int? PerPage,
    [SortableBy(typeof(UserDto), NamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    string? Sort) : IUseCase;
