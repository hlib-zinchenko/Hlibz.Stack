using System.Net.Http.Json;
using System.Text.Json;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The Mcp host's tool errors: each tool lists the errors it can return, with guidance, in its
/// <c>_meta</c>; a failed call says what to do next, with the code and details as structured
/// content. The host doesn't start when a tool's error has no guidance, so starting it checks that.
/// </summary>
public sealed class McpToolErrorTests(StackApiFactory api) : IClassFixture<StackApiFactory>
{
    [Fact]
    public async Task ToolsList_EachTool_ListsItsErrorsWithGuidance()
    {
        StackClient user = await StackClient.SignUpAsync(api);
        await using StackMcpFactory mcp = new(api.Database);

        JsonElement result = await CallAsync(mcp, user, "tools/list", new { });

        JsonElement[] tools = [.. result.GetProperty("tools").EnumerateArray()];
        Assert.All(tools, tool => Assert.All(
            tool.GetProperty("_meta").GetProperty("hlibz/errors").EnumerateArray(),
            error => Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("guidance").GetString()))));
        JsonElement updateItem = Assert.Single(tools, tool => tool.GetProperty("name").GetString() == "update_todo_item");
        Assert.Equal(
            ["stack_1003", "stack_1004", "stack_1010", "validation_failed", "concurrency_conflict"],
            Codes(updateItem));
        JsonElement getList = Assert.Single(tools, tool => tool.GetProperty("name").GetString() == "get_todo_list");
        Assert.Equal(["stack_1003"], Codes(getList));
    }

    [Fact]
    public async Task ToolCall_ThatFails_SaysWhatToDoWithStructuredDetails()
    {
        StackClient user = await StackClient.SignUpAsync(api);
        await using StackMcpFactory mcp = new(api.Database);
        string listId = Guid.NewGuid().ToString();

        JsonElement result = await CallAsync(
            mcp, user, "tools/call", new { name = "get_todo_list", arguments = new { todoListId = listId } });

        Assert.True(result.GetProperty("isError").GetBoolean());
        string? text = result.GetProperty("content")[0].GetProperty("text").GetString();
        Assert.StartsWith($"stack_1003: No todo list with id {listId}", text, StringComparison.Ordinal);
        Assert.Contains("list_todo_lists", text, StringComparison.Ordinal);
        JsonElement structured = result.GetProperty("structuredContent");
        Assert.Equal("stack_1003", structured.GetProperty("code").GetString());
        Assert.Equal(listId, structured.GetProperty("details").GetProperty("todoListId").GetString());
    }

    [Fact]
    public async Task ToolCall_ThatFailsValidation_NamesTheArgumentsInCamelCase()
    {
        StackClient user = await StackClient.SignUpAsync(api);
        await using StackMcpFactory mcp = new(api.Database);

        JsonElement result = await CallAsync(
            mcp, user, "tools/call", new { name = "create_todo_list", arguments = new { title = " " } });

        Assert.True(result.GetProperty("isError").GetBoolean());
        Assert.True(result.GetProperty("structuredContent").GetProperty("details").GetProperty("errors")
            .TryGetProperty("title", out _));
    }

    [Fact]
    public async Task ToolCall_OnAListDeletedMeanwhile_SaysToGetItAgain()
    {
        StackClient user = await StackClient.SignUpAsync(api);
        string listId = StackClient.Id(await user.OkAsync(HttpMethod.Post, "api/todo-lists", new { title = "Groceries" }));
        await using StackMcpFactory mcp = new(api.Database);
        mcp.ConcurrentDelete.Arm(listId);

        JsonElement result = await CallAsync(
            mcp, user, "tools/call", new { name = "rename_todo_list", arguments = new { todoListId = listId, title = "Shopping" } });

        Assert.True(result.GetProperty("isError").GetBoolean());
        Assert.StartsWith(
            "concurrency_conflict: The data changed while this call saved it",
            result.GetProperty("content")[0].GetProperty("text").GetString(),
            StringComparison.Ordinal);
        Assert.Equal("concurrency_conflict", result.GetProperty("structuredContent").GetProperty("code").GetString());
    }

    private static IEnumerable<string?> Codes(JsonElement tool) =>
        tool.GetProperty("_meta").GetProperty("hlibz/errors").EnumerateArray()
            .Select(error => error.GetProperty("code").GetString());

    // Stateless streamable HTTP: one POST, the answer as a server-sent event.
    private static async Task<JsonElement> CallAsync(StackMcpFactory mcp, StackClient user, string method, object parameters)
    {
        using HttpClient http = mcp.CreateClient();
        using HttpRequestMessage request = new(HttpMethod.Post, "mcp")
        {
            Content = JsonContent.Create(new Dictionary<string, object> { ["jsonrpc"] = "2.0", ["id"] = 1, ["method"] = method, ["params"] = parameters }),
        };
        request.Headers.Authorization = user.Http.DefaultRequestHeaders.Authorization;
        request.Headers.Accept.ParseAdd("application/json");
        request.Headers.Accept.ParseAdd("text/event-stream");

        using HttpResponseMessage response = await http.SendAsync(request, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        string data = body.Split('\n').First(line => line.StartsWith("data: ", StringComparison.Ordinal))["data: ".Length..];
        return JsonElement.Parse(data).GetProperty("result");
    }
}
