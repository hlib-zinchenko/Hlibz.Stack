using System.Net.Http.Json;
using System.Text.Json;

namespace Hlibz.Stack.Tests;

/// <summary>
/// What the OpenAPI documents say about errors: each declared error with its own body, the ambient
/// errors sent with the same status code beside them, and a code on every body.
/// </summary>
public sealed class OpenApiDocumentTests(StackApiFactory factory) : IClassFixture<StackApiFactory>
{
    [Fact]
    public async Task ErrorResponse_Code_IsARequiredString()
    {
        JsonElement document = await DocumentAsync("stack");

        JsonElement error = document.GetProperty("components").GetProperty("schemas").GetProperty("ErrorResponse");
        Assert.Equal("string", error.GetProperty("properties").GetProperty("code").GetProperty("type").GetString());
        Assert.Contains("code", error.GetProperty("required").EnumerateArray().Select(name => name.GetString()));
    }

    [Fact]
    public async Task Conflict_OfAUseCase_ListsItsErrorsAndTheConcurrencyConflict()
    {
        JsonElement document = await DocumentAsync("stack");

        Assert.Equal(
            ["TodoListTitleTakenErrorResponse", "TodoListLimitReachedErrorResponse", "ConcurrencyConflictErrorResponse"],
            OneOf(Body(document, "/api/todo-lists", "post", "409")));
    }

    [Fact]
    public async Task BadRequest_OfAUseCaseDeclaringOne_ListsValidationFailedToo()
    {
        JsonElement document = await DocumentAsync("admin");

        Assert.Equal(
            ["OwnRoleChangeErrorResponse", "ValidationFailedErrorResponse"],
            OneOf(Body(document, "/api/admin/users/{userId}/role", "put", "400")));
    }

    [Fact]
    public async Task Export_ThatSendsAFile_DocumentsItsUseCasesErrors()
    {
        JsonElement document = await DocumentAsync("stack");

        JsonElement responses = Operation(document, "/api/todo-lists/{id}/export", "get").GetProperty("responses");
        Assert.True(responses.GetProperty("200").GetProperty("content").TryGetProperty("text/markdown", out _));
        Assert.Equal(
            "#/components/schemas/TodoListNotFoundErrorResponse",
            Body(document, "/api/todo-lists/{id}/export", "get", "404").GetProperty("$ref").GetString());
    }

    [Fact]
    public async Task Auth_RateLimited_DocumentsTooManyRequests()
    {
        JsonElement document = await DocumentAsync("stack");

        Assert.Equal(
            "#/components/schemas/ErrorResponse",
            Body(document, "/api/auth/login", "post", "429").GetProperty("$ref").GetString());
    }

    private static JsonElement Operation(JsonElement document, string path, string method) =>
        document.GetProperty("paths").GetProperty(path).GetProperty(method);

    private static JsonElement Body(JsonElement document, string path, string method, string status) =>
        Operation(document, path, method).GetProperty("responses").GetProperty(status)
            .GetProperty("content").GetProperty("application/json").GetProperty("schema");

    private static string[] OneOf(JsonElement schema) =>
        [.. schema.GetProperty("oneOf").EnumerateArray().Select(option => (option.GetProperty("$ref").GetString() ?? string.Empty).Split('/')[^1])];

    private async Task<JsonElement> DocumentAsync(string name)
    {
        using HttpClient http = factory.CreateClient();
        return await http.GetFromJsonAsync<JsonElement>($"openapi/{name}.json", TestContext.Current.CancellationToken);
    }
}
