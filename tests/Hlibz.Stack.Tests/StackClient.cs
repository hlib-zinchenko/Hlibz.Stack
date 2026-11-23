using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Hlibz.Stack.Tests;

/// <summary>Calls the Api as one signed-in user, in its snake_case JSON.</summary>
public sealed class StackClient(HttpClient http)
{
    public const string AdminEmail = "admin@stack.test";
    public const string Password = "Passw0rd!";

    public HttpClient Http { get; } = http;

    public static async Task<StackClient> SignUpAsync(StackApiFactory factory)
    {
        HttpClient http = factory.CreateClient();
        string email = $"{Guid.NewGuid():N}@stack.test";
        using HttpResponseMessage registered = await http.PostAsJsonAsync(
            "api/auth/register",
            new { email, name = "Test", password = Password },
            TestContext.Current.CancellationToken);
        registered.EnsureSuccessStatusCode();
        return await LogInAsync(http, email);
    }

    public static Task<StackClient> AdminAsync(StackApiFactory factory) =>
        LogInAsync(factory.CreateClient(), AdminEmail);

    public static string Id(JsonElement element) =>
        element.GetProperty("id").GetString() ?? throw new InvalidOperationException("No id.");

    public async Task<(HttpStatusCode Status, JsonElement Body)> SendAsync(
        HttpMethod method,
        string path,
        object? body = null)
    {
        using HttpRequestMessage request = new(method, path);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using HttpResponseMessage response = await Http.SendAsync(request, TestContext.Current.CancellationToken);
        string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return (response.StatusCode, json.Length == 0 ? default : JsonElement.Parse(json));
    }

    public async Task<JsonElement> OkAsync(HttpMethod method, string path, object? body = null)
    {
        (HttpStatusCode status, JsonElement json) = await SendAsync(method, path, body);
        Assert.Equal(HttpStatusCode.OK, status);
        return json;
    }

    private static async Task<StackClient> LogInAsync(HttpClient http, string email)
    {
        using HttpResponseMessage response = await http.PostAsJsonAsync(
            "api/auth/login",
            new { email, password = Password },
            TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        JsonElement token = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer", token.GetProperty("access_token").GetString());
        return new StackClient(http);
    }
}
