namespace Hlibz.Stack.Mcp.Tools;

/// <summary>
/// Turns a use case's failure into a tool error: an <see cref="McpException"/>, whose message the
/// client (and the model) sees, with every validation error.
/// </summary>
internal static class ToolResults
{
    public static T ValueOrThrow<T>(this Result<T> result) =>
        result is { IsSuccess: true, Value: { } value } ? value : throw Failure(result);

    public static void ThrowIfFailed(this Result result)
    {
        if (!result.IsSuccess)
        {
            throw Failure(result);
        }
    }

    private static McpException Failure(Result result)
    {
        string message = $"{result.Status}: {result.ErrorMessage ?? Describe(result.Status)}";
        if (result.ValidationErrors is { Count: > 0 } errors)
        {
            message += " " + string.Join(
                " ",
                errors.Select(error => $"{error.Key}: {string.Join(" ", error.Value)}"));
        }

        return new McpException(message);
    }

    private static string Describe(ResultStatus status) => status switch
    {
        ResultStatus.Unauthorized => "Send an access token from the Api's POST api/auth/login.",
        ResultStatus.Forbidden => "Only admins may do this.",
        _ => "The request failed.",
    };
}
