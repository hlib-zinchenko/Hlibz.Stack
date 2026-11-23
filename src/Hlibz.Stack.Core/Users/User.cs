using Hlibz.Stack.Core.Entities;

namespace Hlibz.Stack.Core.Users;

/// <summary>Someone who signs in: owns todo lists, and as an admin manages everyone's.</summary>
public sealed class User : Entity<UserId>
{
    /// <summary>The longest email address.</summary>
    public const int EmailMaxLength = 254;

    /// <summary>The longest name.</summary>
    public const int NameMaxLength = 100;

    /// <summary>The longest stored password hash.</summary>
    public const int PasswordHashMaxLength = 200;

    private User(UserId id, string email, string name, string passwordHash, Role role)
        : base(id)
    {
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
        Role = role;
    }

    /// <summary>Gets the email address, trimmed and lower-cased: unique, and what users sign in with.</summary>
    public string Email { get; private set; }

    /// <summary>Gets the name the user goes by.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the hash of the user's password.</summary>
    public string PasswordHash { get; private set; }

    /// <summary>Gets what the user may do.</summary>
    public Role Role { get; private set; }

    /// <summary>Creates a user.</summary>
    /// <param name="email">The email address, normalized here.</param>
    /// <param name="name">The name, trimmed here.</param>
    /// <param name="passwordHash">The hash of the user's password.</param>
    /// <param name="role">What the user may do.</param>
    /// <returns>The user.</returns>
    public static User Create(string email, string name, string passwordHash, Role role) =>
        new(EntityId.New<UserId>(), NormalizeEmail(email), name.Trim(), passwordHash, role);

    /// <summary>Normalizes an email address the way it's stored and looked up.</summary>
    /// <param name="email">The address as typed.</param>
    /// <returns>The address trimmed and lower-cased.</returns>
    [SuppressMessage(
        "Globalization",
        "CA1308:Normalize strings to uppercase",
        Justification = "Email addresses are shown as stored, and read better in lower case.")]
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    /// <summary>Changes what the user may do.</summary>
    /// <param name="role">The new role.</param>
    public void ChangeRole(Role role) => Role = role;

    /// <summary>Replaces the stored password hash, after a rehash.</summary>
    /// <param name="passwordHash">The new hash.</param>
    public void ChangePasswordHash(string passwordHash) => PasswordHash = passwordHash;
}
