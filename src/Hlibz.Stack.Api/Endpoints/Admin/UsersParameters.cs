using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.Users.GetUsers;

namespace Hlibz.Stack.Api.Endpoints.Admin;

/// <summary>A list of users' query string.</summary>
internal sealed class UsersParameters : ListParameters
{
    /// <summary>Gets the order.</summary>
    [FromQuery(Name = "sort")]
    [SortableBy(typeof(UserDto), NamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
    public string? Sort { get; init; }

    /// <summary>Gets the use case.</summary>
    /// <returns>The use case.</returns>
    public GetUsersQuery ToQuery() => new(Search, Page, PerPage, Sort);
}
