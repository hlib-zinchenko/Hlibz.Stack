namespace Hlibz.Stack.Application.UseCases.Users.LogIn;

/// <summary>Signs a user in, for an access token.</summary>
/// <param name="Email">The email address the user signed up with.</param>
/// <param name="Password">The user's password.</param>
[Anonymous]
public sealed record LogInCommand(
    [Required] string Email,
    [Required] string Password) : IUseCase
{
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(LogInCommand)} {{ Email = {Email} }}";
}
