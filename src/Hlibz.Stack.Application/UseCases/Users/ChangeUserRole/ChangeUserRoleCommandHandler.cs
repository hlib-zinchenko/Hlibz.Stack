using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

internal sealed class ChangeUserRoleCommandHandler(
    ICallerResolver callers,
    IUserRepository users,
    IUnitOfWork unitOfWork) : IChangeUserRoleCommandHandler
{
    public async Task<Result<UserDto>> HandleAsync(
        ChangeUserRoleCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        if (useCase.UserId == caller.Id)
        {
            return Result<UserDto>.Error(StackErrors.OwnRoleChange);
        }

        if (await users.GetByIdAsync(useCase.UserId, cancellationToken) is not { } user)
        {
            return Result<UserDto>.NotFound(StackErrors.UserNotFound);
        }

        user.ChangeRole(useCase.Change.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UserDto>.Success(UserDto.From(user));
    }
}
