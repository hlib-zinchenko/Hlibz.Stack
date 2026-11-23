namespace Hlibz.Stack.Application.Dto;

/// <summary>An access token, sent as <c>Authorization: Bearer {token}</c>.</summary>
public sealed class AccessTokenDto
{
    /// <summary>Gets the token.</summary>
    public required string AccessToken { get; init; }

    /// <summary>Gets when the token stops working.</summary>
    public required DateTimeOffset ExpiresAt { get; init; }
}
