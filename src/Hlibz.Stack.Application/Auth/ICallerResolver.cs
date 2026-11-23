namespace Hlibz.Stack.Application.Auth;

/// <summary>Finds the user running the use case, once per scope.</summary>
internal interface ICallerResolver
{
    /// <summary>Gets the caller.</summary>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <returns>The caller; <see langword="null"/> when nobody, or no existing user, is signed in.</returns>
    Task<Caller?> GetAsync(CancellationToken cancellationToken);

    /// <summary>Gets the caller of a use case the authorization filter let through.</summary>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <returns>The caller.</returns>
    /// <exception cref="InvalidOperationException">Nobody is signed in.</exception>
    Task<Caller> GetRequiredAsync(CancellationToken cancellationToken);
}
