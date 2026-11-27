// The Hlibz stack's HTTP host. The use cases (Hlibz.Stack.Application) hold every rule; this app
// binds HTTP to them, sends their Results, and documents what they check.
// Run it and open http://localhost:5400/api-docs.
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using Hlibz.OpenApi;
using Hlibz.Redoc.Extensions;
using Hlibz.Stack.Api;
using Hlibz.Stack.Api.Endpoints;
using Hlibz.Stack.Api.Endpoints.Auth;
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

// One error body (ErrorResponse) for every failure: a use case's declared errors, validation's, and
// the auth gate's. Each declared error is documented with its own code and details (AddResults).
builder.Services.AddResultErrorResponses();

// Logging in and signing up are limited per client address (RateLimits:Auth:PermitLimit calls a
// minute). A call turned away gets the ambient RateLimited, 429 rate_limited, in the same body as
// any other error, with Retry-After.
int authPermitLimit = builder.Configuration.GetValue("RateLimits:Auth:PermitLimit", 10);
builder.Services.AddRateLimiter(limiter =>
{
    limiter.AddPolicy(AuthEndpointGroup.RateLimitPolicy, context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        _ => new FixedWindowRateLimiterOptions { PermitLimit = authPermitLimit, Window = TimeSpan.FromMinutes(1) }));
    limiter.OnRejected = (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
        }

        return new ValueTask(context.HttpContext.Response.WriteErrorAsync(
            Result.Failure(new RateLimited()),
            cancellationToken));
    };
});

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
app.UseRateLimiter();
app.UseOutputCache();
app.MapOpenApi().CacheOutput();

// Every endpoint returns its use case's Result as is. WithResultResponses documents the errors its
// handler's error set declares; Endpoints adds 401/403 where a group requires authorization, 429
// where it's rate limited.
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
        docs.EnableTryIt();
        docs.PickerPlacement = ReDocToolbarPlacement.Sidebar;
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
    .AddResults()
    .AddPaging()
    .AddDescriptionCleanup()
    .AddBearerSecurity(bearer => bearer.Description =
        "The access token from POST api/auth/login, sent as 'Authorization: Bearer {token}'.");
