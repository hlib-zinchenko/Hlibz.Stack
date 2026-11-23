namespace Hlibz.Stack.Application.UseCases.Users.RegisterUser;

/// <summary>Signs a new user up, as a <see cref="Role.User"/>.</summary>
/// <param name="Email">The email address to sign in with.</param>
/// <param name="Name">The name to go by.</param>
/// <param name="Password">The password to sign in with.</param>
[Anonymous]
public sealed record RegisterUserCommand(
    [Required, EmailAddress, MaxLength(User.EmailMaxLength)] string Email,
    [Required, MaxLength(User.NameMaxLength)] string Name,
    [Required, StringLength(128, MinimumLength = 8)] string Password) : IUseCase
{
    /// <inheritdoc/>
    public override string ToString() => $"{nameof(RegisterUserCommand)} {{ Email = {Email} }}";
}
