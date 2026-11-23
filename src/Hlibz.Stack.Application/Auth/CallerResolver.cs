namespace Hlibz.Stack.Application.Auth;

/// <summary>
/// Reads the caller's role from the database rather than from the token, so a role change applies
/// to the next request.
/// </summary>
/// <param name="currentUser">Who the host says is calling.</param>
/// <param name="users">Loads the caller.</param>
internal sealed class CallerResolver(ICurrentUser currentUser, IUserRepository users) : ICallerResolver
{
    private Task<Caller?>? _caller;

    public Task<Caller?> GetAsync(CancellationToken cancellationToken) =>
        _caller ??= LoadAsync(cancellationToken);

    public async Task<Caller> GetRequiredAsync(CancellationToken cancellationToken) =>
        await GetAsync(cancellationToken)
        ?? throw new InvalidOperationException(
            "The use case ran without a signed-in user: mark it [Anonymous], or register the "
            + "authorization filter.");

    private async Task<Caller?> LoadAsync(CancellationToken cancellationToken) =>
        currentUser.Id is { } id && await users.GetByIdAsync(id, cancellationToken) is { } user
            ? new Caller(user.Id, user.Role)
            : null;
}
