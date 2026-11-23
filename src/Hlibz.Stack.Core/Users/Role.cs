namespace Hlibz.Stack.Core.Users;

/// <summary>What a user may do.</summary>
public enum Role
{
    /// <summary>Manages their own todo lists.</summary>
    User,

    /// <summary>Manages every user's todo lists, and users' roles.</summary>
    Admin,
}
