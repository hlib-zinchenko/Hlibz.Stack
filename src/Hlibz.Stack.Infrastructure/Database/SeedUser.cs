namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>A user an empty database starts with.</summary>
public sealed class SeedUser
{
    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the role.</summary>
    public Role Role { get; set; }
}
