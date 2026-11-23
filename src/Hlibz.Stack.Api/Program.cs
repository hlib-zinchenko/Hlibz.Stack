// The Hlibz stack's HTTP host. The use cases (Hlibz.Stack.Application) hold every rule; this app
// binds HTTP to them, sends their Results, and documents what they check.
// Run it and open http://localhost:5400/api-docs.
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

using Hlibz.OpenApi;
using Hlibz.Redoc.Extensions;
using Hlibz.Stack.Api;
using Hlibz.Stack.Api.Endpoints;
using Hlibz.Stack.Application;
using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Core.Entities;
using Hlibz.Stack.Infrastructure;
using Hlibz.Stack.Infrastructure.Auth;
using Hlibz.Stack.Infrastructure.Database;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddStackApplication()
    .AddStackInfrastructure(builder.Configuration)
    .AddHttpContextAccessor()
    .AddScoped<ICurrentUser, HttpCurrentUser>();

// Who's calling: the access token from POST api/auth/login. What they may do is the use cases'
// business (AuthorizationFilter), so HTTP only checks there's a valid token.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.MapInboundClaims = false);
builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
        bearer.TokenValidationParameters = jwt.Value.CreateValidationParameters());
builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// One error body (ErrorResponse) for every failure: a use case's, validation's, and the auth gate's.
builder.Services.AddResultErrorResponses();

// One document for the app, one for the admin endpoints; Microsoft.AspNetCore.OpenApi builds a
// document again on every request, so it's cached.
builder.Services.AddOutputCache();
foreach (string document in StackDocuments.All)
{
    builder.Services.AddOpenApi(document, ConfigureDocument);
}

WebApplication app = builder.Build();

// The build-time OpenAPI tool (GetDocument.Insider) starts the app without a database.
if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
{
    await StackDatabase.InitializeAsync(app.Services);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
app.MapOpenApi().CacheOutput();

// Every endpoint returns its use case's Result as is; ErrorResponse is the documented error body.
app.MapEndpoints(typeof(Program).Assembly, options => options
    .UseErrorResponse<ErrorResponse>()
    .ConfigureGroups(group => group.Builder.WithGroupName(
        group.Group is IAdminEndpointGroup ? StackDocuments.Admin : StackDocuments.App))
    .ConfigureGroups<IAdminEndpointGroup>((_, group) => group.RequireAuthorization())
    .ConfigureEndpoints(endpoint => endpoint.Builder.WithResultResponses()));

app.MapReDocDocuments(
    [
        new ReDocDocument(StackDocuments.App, "Hlibz Stack"),
        new ReDocDocument(StackDocuments.Admin, "Hlibz Stack (Admin)"),
    ],
    docs =>
    {
        docs.DefaultColorMode = ReDocColorMode.System;
        docs.AdditionalConfigJson = """{ "showExtensions": ["x-rules"] }""";
    });
app.MapGet("/", () => Results.Redirect("/api-docs")).ExcludeFromDescription();

await app.RunAsync();

static void ConfigureDocument(OpenApiOptions options) => options
    .AddStronglyTypedIds(ids =>
    {
        ids.ValueTypeOf = type => typeof(IEntityId).IsAssignableFrom(type) ? typeof(Guid) : null;
        ids.AsComponents = true;
    })
    .AddXmlDocumentation()
    .AddOpenApiRules()
    .AddUseCases(useCases => useCases.ValidationResponseType = typeof(ErrorResponse))
    .AddPaging()
    .AddDescriptionCleanup()
    .AddBearerSecurity(bearer => bearer.Description =
        "The access token from POST api/auth/login, sent as 'Authorization: Bearer {token}'.");
