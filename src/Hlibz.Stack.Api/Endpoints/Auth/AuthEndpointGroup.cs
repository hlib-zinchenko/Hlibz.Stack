namespace Hlibz.Stack.Api.Endpoints.Auth;

/// <summary>Signing up and logging in: anonymous, and rate limited per client address.</summary>
internal sealed class AuthEndpointGroup : IEndpointGroup
{
    /// <summary>The rate limiting policy of the group: <c>RateLimits:Auth:PermitLimit</c> a minute.</summary>
    public const string RateLimitPolicy = "auth";

    public string Prefix => "api/auth";

    public void Configure(RouteGroupBuilder group) =>
        group.WithTags("Auth").RequireRateLimiting(RateLimitPolicy);
}
