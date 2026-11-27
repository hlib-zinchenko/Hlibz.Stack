using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.LogIn;

internal sealed class LogInCommandHandler(
    IUserRepository users,
    PasswordHasher passwords,
    IAccessTokenIssuer tokens) : ILogInCommandHandler
{
    public async Task<Result<AccessTokenDto, LogInError>> HandleAsync(
        LogInCommand useCase,
        CancellationToken cancellationToken = default)
    {
        User? user = await users.GetByEmailAsync(User.NormalizeEmail(useCase.Email), cancellationToken);

        // Verified even without a user, so the time taken doesn't say whether the address exists.
        return passwords.Verify(user?.PasswordHash, useCase.Password) && user is not null
            ? tokens.Issue(user)
            : new InvalidCredentials();
    }
}
