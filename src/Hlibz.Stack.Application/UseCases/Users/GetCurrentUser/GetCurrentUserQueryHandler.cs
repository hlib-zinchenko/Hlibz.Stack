using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler(ICallerResolver callers, IUserRepository users)
    : IGetCurrentUserQueryHandler
{
    public async Task<Result<UserDto>> HandleAsync(
        GetCurrentUserQuery useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        return await users.GetByIdAsync(caller.Id, cancellationToken) is { } user
            ? Result<UserDto>.Success(UserDto.From(user))
            : Result<UserDto>.NotFound(StackErrors.UserNotFound);
    }
}
