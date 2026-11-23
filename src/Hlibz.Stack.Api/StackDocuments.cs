namespace Hlibz.Stack.Api;

/// <summary>The OpenAPI documents, by name: <c>/openapi/{name}.json</c>.</summary>
internal static class StackDocuments
{
    /// <summary>Everything a signed-in user, or a visitor, calls.</summary>
    public const string App = "stack";

    /// <summary>The admin endpoints.</summary>
    public const string Admin = "admin";

    public static readonly string[] All = [App, Admin];
}
