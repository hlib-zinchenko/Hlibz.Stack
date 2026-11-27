using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The errors no use case declares, which the layers around them add: the auth gate's, the rate
/// limiter's, a save whose rows changed meanwhile. Same body as any error, each with its code.
/// </summary>
public sealed class AmbientErrorTests(StackApiFactory factory) : IClassFixture<StackApiFactory>
{
    [Fact]
    public async Task AnyEndpoint_WithoutAToken_IsUnauthorizedWithItsCode()
    {
        StackClient anonymous = new(factory.CreateClient());

        (HttpStatusCode status, JsonElement body) = await anonymous.SendAsync(HttpMethod.Get, "api/todo-lists");

        Assert.Equal(HttpStatusCode.Unauthorized, status);
        Assert.Equal("unauthorized", body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task AdminEndpoint_AsAUser_IsForbiddenWithItsCode()
    {
        StackClient user = await StackClient.SignUpAsync(factory);

        (HttpStatusCode status, JsonElement body) = await user.SendAsync(HttpMethod.Get, "api/admin/users");

        Assert.Equal(HttpStatusCode.Forbidden, status);
        Assert.Equal("forbidden", body.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Rename_OfAListDeletedMeanwhile_IsAConcurrencyConflict()
    {
        StackClient user = await StackClient.SignUpAsync(factory);
        string listId = StackClient.Id(await user.OkAsync(HttpMethod.Post, "api/todo-lists", new { title = "Groceries" }));
        factory.ConcurrentDelete.Arm(listId);

        (HttpStatusCode status, JsonElement body) = await user.SendAsync(
            HttpMethod.Put, $"api/todo-lists/{listId}", new { title = "Shopping" });

        Assert.Equal(HttpStatusCode.Conflict, status);
        Assert.Equal("concurrency_conflict", body.GetProperty("code").GetString());
        Assert.False(body.TryGetProperty("details", out _));
        (HttpStatusCode gone, _) = await user.SendAsync(HttpMethod.Get, $"api/todo-lists/{listId}");
        Assert.Equal(HttpStatusCode.NotFound, gone);
    }

    [Fact]
    public async Task LogIn_BeyondTheRateLimit_IsRateLimitedWithRetryAfter()
    {
        await using WebApplicationFactory<Program> limited = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("RateLimits:Auth:PermitLimit", "2"));
        using HttpClient http = limited.CreateClient();
        object wrong = new { email = StackClient.AdminEmail, password = "wrong" };

        using HttpResponseMessage first = await http.PostAsJsonAsync("api/auth/login", wrong, TestContext.Current.CancellationToken);
        using HttpResponseMessage second = await http.PostAsJsonAsync("api/auth/login", wrong, TestContext.Current.CancellationToken);
        using HttpResponseMessage third = await http.PostAsJsonAsync("api/auth/login", wrong, TestContext.Current.CancellationToken);
        JsonElement body = await third.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, third.StatusCode);
        Assert.Equal("rate_limited", body.GetProperty("code").GetString());
        Assert.NotNull(third.Headers.RetryAfter?.Delta);
    }
}
