using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Hlibz.Stack.Infrastructure.Auth;

/// <summary>The access tokens the Api issues and both hosts accept (section <c>Auth:Jwt</c>).</summary>
public sealed class JwtOptions
{
    /// <summary>The configuration section.</summary>
    public const string SectionName = "Auth:Jwt";

    /// <summary>Gets or sets who issues the tokens.</summary>
    public string Issuer { get; set; } = "hlibz-stack";

    /// <summary>Gets or sets who the tokens are for.</summary>
    public string Audience { get; set; } = "hlibz-stack";

    /// <summary>Gets or sets the HMAC key the tokens are signed with: at least 32 bytes, kept secret.</summary>
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>Gets or sets how long a token works.</summary>
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromHours(8);

    /// <summary>Gets the key the tokens are signed with.</summary>
    public SymmetricSecurityKey Key => new(Encoding.UTF8.GetBytes(SigningKey));

    /// <summary>Gets how a host checks a token: issuer, audience, lifetime and signature.</summary>
    /// <returns>The parameters, for the JWT bearer handler.</returns>
    public TokenValidationParameters CreateValidationParameters() => new()
    {
        ValidIssuer = Issuer,
        ValidAudience = Audience,
        IssuerSigningKey = Key,
        NameClaimType = JwtClaims.Name,
    };
}
