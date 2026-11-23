using System.Net;
using System.Text.Json;

namespace Hlibz.Stack.Tests;

public sealed class TodoListTests(StackApiFactory factory) : IClassFixture<StackApiFactory>
{
    [Fact]
    public async Task List_WithAnItemDone_CountsItAndStampsTheList()
    {
        StackClient user = await StackClient.SignUpAsync(factory);
        string listId = await CreateListAsync(user, "Groceries");
        JsonElement item = await user.OkAsync(HttpMethod.Post, $"api/todo-lists/{listId}/items", new { title = "Milk" });
        await user.OkAsync(
            HttpMethod.Patch,
            $"api/todo-lists/{listId}/items/{item.GetProperty("id").GetString()}",
            new { is_done = true });

        JsonElement lists = await user.OkAsync(HttpMethod.Get, "api/todo-lists");

        JsonElement list = Assert.Single(lists.GetProperty("data").EnumerateArray());
        Assert.Equal(1, list.GetProperty("item_count").GetInt32());
        Assert.Equal(1, list.GetProperty("done_count").GetInt32());
        Assert.Equal(JsonValueKind.String, list.GetProperty("updated_at").ValueKind);
    }

    [Fact]
    public async Task List_OfAnotherUser_IsNotFound()
    {
        StackClient owner = await StackClient.SignUpAsync(factory);
        StackClient other = await StackClient.SignUpAsync(factory);
        string listId = await CreateListAsync(owner, "Mine");

        (HttpStatusCode status, JsonElement body) = await other.SendAsync(HttpMethod.Get, $"api/todo-lists/{listId}");

        Assert.Equal(HttpStatusCode.NotFound, status);
        Assert.Equal("stack_1003", body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task List_OfAnotherUser_IsManagedByAnAdmin()
    {
        StackClient owner = await StackClient.SignUpAsync(factory);
        StackClient admin = await StackClient.AdminAsync(factory);
        string listId = await CreateListAsync(owner, "Mine");
        string ownerId = StackClient.Id(await owner.OkAsync(HttpMethod.Get, "api/users/me"));

        JsonElement ownersLists = await admin.OkAsync(HttpMethod.Get, $"api/admin/users/{ownerId}/todo-lists");
        await admin.OkAsync(HttpMethod.Put, $"api/todo-lists/{listId}", new { title = "Renamed by an admin" });

        Assert.Equal(listId, Assert.Single(ownersLists.GetProperty("data").EnumerateArray()).GetProperty("id").GetString());
        JsonElement list = await owner.OkAsync(HttpMethod.Get, $"api/todo-lists/{listId}");
        Assert.Equal("Renamed by an admin", list.GetProperty("title").GetString());
    }

    [Fact]
    public async Task AdminEndpoint_AsAUser_IsForbidden()
    {
        StackClient user = await StackClient.SignUpAsync(factory);

        (HttpStatusCode status, _) = await user.SendAsync(HttpMethod.Get, "api/admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, status);
    }

    [Fact]
    public async Task Create_WithABlankTitle_FailsValidationOnTitle()
    {
        StackClient user = await StackClient.SignUpAsync(factory);

        (HttpStatusCode status, JsonElement body) = await user.SendAsync(HttpMethod.Post, "api/todo-lists", new { title = " " });

        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.True(body.GetProperty("validation_errors").TryGetProperty("title", out _));
    }

    [Fact]
    public async Task List_WithAnUnknownSort_FailsValidationOnSort()
    {
        StackClient user = await StackClient.SignUpAsync(factory);

        (HttpStatusCode status, JsonElement body) = await user.SendAsync(HttpMethod.Get, "api/todo-lists?sort=owner");

        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.Contains(
            "item_count",
            body.GetProperty("validation_errors").GetProperty("sort")[0].GetString(),
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task ChangeRole_OfOneself_IsRejected()
    {
        StackClient admin = await StackClient.AdminAsync(factory);
        string adminId = StackClient.Id(await admin.OkAsync(HttpMethod.Get, "api/users/me"));

        (HttpStatusCode status, JsonElement body) = await admin.SendAsync(
            HttpMethod.Put, $"api/admin/users/{adminId}/role", new { role = "User" });

        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.Equal("stack_1006", body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task AnyEndpoint_WithoutAToken_IsUnauthorized()
    {
        using HttpClient anonymous = factory.CreateClient();

        using HttpResponseMessage response = await anonymous.GetAsync(
            "api/todo-lists", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<string> CreateListAsync(StackClient client, string title) =>
        StackClient.Id(await client.OkAsync(HttpMethod.Post, "api/todo-lists", new { title }));
}
