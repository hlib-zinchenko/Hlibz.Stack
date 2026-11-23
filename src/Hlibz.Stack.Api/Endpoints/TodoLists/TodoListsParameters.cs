using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;

namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>A list of todo lists' query string.</summary>
internal sealed class TodoListsParameters : ListParameters
{
    /// <summary>Gets the order.</summary>
    [FromQuery(Name = "sort")]
    [SortableBy(typeof(TodoListDto), NamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    public string? Sort { get; init; }

    /// <summary>Gets the use case for <paramref name="ownerId"/>'s lists.</summary>
    /// <param name="ownerId">The user; <see langword="null"/> for the signed-in user.</param>
    /// <returns>The use case.</returns>
    public GetTodoListsQuery ToQuery(UserId? ownerId) =>
        new(ownerId, Search, Page, PerPage, Sort);
}
