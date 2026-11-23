using System.Security.Claims;

using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Application.Dto;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Hlibz.Stack.Infrastructure.Auth;

/// <summary>
/// Issues HS256 tokens naming the user. Their role isn't in it: the use cases read the role from
/// the database on every call, so a change applies at once.
/// </summary>
/// <param name="options">The token settings.</param>
/// <param name="time">The clock.</param>
internal sealed class JwtAccessTokenIssuer(IOptions<JwtOptions> options, TimeProvider time)
    : IAccessTokenIssuer
{
    private readonly JsonWebTokenHandler _handler = new();

    public AccessTokenDto Issue(User user)
    {
        JwtOptions jwt = options.Value;
        DateTimeOffset now = time.GetUtcNow();
        DateTimeOffset expiresAt = now + jwt.AccessTokenLifetime;
        string token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now.UtcDateTime,
            NotBefore = now.UtcDateTime,
            Expires = expiresAt.UtcDateTime,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtClaims.Subject, user.Id.Value.ToString()),
                new Claim(JwtClaims.Name, user.Name),
                new Claim(JwtClaims.Email, user.Email),
            ]),
            SigningCredentials = new SigningCredentials(jwt.Key, SecurityAlgorithms.HmacSha256),
        });
        return new AccessTokenDto { AccessToken = token, ExpiresAt = expiresAt };
    }
}
