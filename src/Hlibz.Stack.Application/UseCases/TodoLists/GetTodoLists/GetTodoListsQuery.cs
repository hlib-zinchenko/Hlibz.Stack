using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

/// <summary>Lists a user's todo lists, one page at a time.</summary>
/// <param name="OwnerId">The user; the signed-in user when left out. Only admins may name another.</param>
/// <param name="Search">Text the title contains.</param>
/// <param name="Page">The page, from 1.</param>
/// <param name="PerPage">Lists per page.</param>
/// <param name="Sort">The order.</param>
public sealed record GetTodoListsQuery(
    UserId? OwnerId,
    string? Search,
    [Range(1, int.MaxValue)] int? Page,
    [Range(1, 100)] int? PerPage,
    [SortableBy(typeof(TodoListDto), NamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    string? Sort) : IUseCase;
