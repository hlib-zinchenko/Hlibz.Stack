// The Hlibz stack's MCP host: the use cases the Api serves, as MCP tools.
// Run it and open http://localhost:5401/mcp-docs; connect a client to http://localhost:5401/mcp
// with the access token from the Api's POST api/auth/login.
using System.Reflection;

using Hlibz.McpDocs.AspNetCore;
using Hlibz.Stack.Application;
using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Infrastructure;
using Hlibz.Stack.Infrastructure.Auth;
using Hlibz.Stack.Infrastructure.Database;
using Hlibz.Stack.Mcp;
using Hlibz.Stack.Mcp.Errors;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddStackApplication()
    .AddStackInfrastructure(builder.Configuration)
    .AddHttpContextAccessor()
    .AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.MapInboundClaims = false);
builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
        bearer.TokenValidationParameters = jwt.Value.CreateValidationParameters());
builder.Services.AddAuthorization();

// Stateless: every request stands alone, so each tool call runs in its own request's scope and
// reads its own access token.
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new() { Name = "hlibz-stack", Title = "Hlibz Stack", Version = "0.1.0" };
        options.ServerInstructions =
            "Todo lists. list_todo_lists first to find a list's id; admins also have the admin_ tools.";
    })
    .WithHttpTransport(options => options.Stateless = true)
    .WithToolsFromAssembly(Assembly.GetExecutingAssembly())
    .WithToolErrors(StackGuidance.Configure);

// A failed tool says what to do next (Errors/StackGuidance.cs), and each tool lists the errors it
// can return, with that guidance, in its _meta, which the docs page lists under Errors
// (Hlibz.Results.Mcp). The errors come from the use case each tool runs; the host doesn't start
// while one has no guidance.
builder.Services.AddSingleton<IToolErrorSource, UseCaseToolErrors>();

WebApplication app = builder.Build();
await StackDatabase.InitializeAsync(app.Services);

app.UseAuthentication();
app.UseAuthorization();

app.MapMcp("/mcp").RequireAuthorization();
app.MapMcpDocs("/mcp-docs", docs =>
{
    docs.Description =
        "Todo lists from any MCP client: the same use cases, rules and errors as the HTTP API.";
    docs.AuthHeader = "Authorization";
    docs.AuthHint = "Log in with the Api's `POST api/auth/login` (http://localhost:5400/api-docs).";
    docs.EnableTryIt();
});
app.MapGet("/", () => Results.Redirect("/mcp-docs/"));

await app.RunAsync();
