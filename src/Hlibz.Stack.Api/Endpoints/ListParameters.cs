using System.ComponentModel.DataAnnotations;

namespace Hlibz.Stack.Api.Endpoints;

/// <summary>
/// A list's query string in the API's snake_case. The use case it's turned into checks it; the
/// rules here only document it.
/// </summary>
internal abstract class ListParameters
{
    /// <summary>Gets text to search for.</summary>
    [FromQuery(Name = "search")]
    public string? Search { get; init; }

    /// <summary>Gets the page, from 1.</summary>
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue)]
    public int? Page { get; init; }

    /// <summary>Gets the items per page; 100 when left out.</summary>
    [FromQuery(Name = "per_page")]
    [Range(1, 100)]
    public int? PerPage { get; init; }
}
