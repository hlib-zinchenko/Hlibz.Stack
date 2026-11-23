using Hlibz.Stack.Core.Users;

namespace Hlibz.Stack.Core.Repositories;

/// <summary>Loads users to change them, and stages new ones.</summary>
public interface IUserRepository
{
    /// <summary>Loads a user.</summary>
    /// <param name="id">The user's id.</param>
    /// <param name="cancellationToken">Cancels the load.</param>
    /// <returns>The user; <see langword="null"/> when there's none with that id.</returns>
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);

    /// <summary>Loads a user by email address.</summary>
    /// <param name="email">The address, normalized with <see cref="User.NormalizeEmail"/>.</param>
    /// <param name="cancellationToken">Cancels the load.</param>
    /// <returns>The user; <see langword="null"/> when there's none with that address.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>Stages a new user, saved with the unit of work.</summary>
    /// <param name="user">The user.</param>
    void Add(User user);
}
