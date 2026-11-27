using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

internal sealed class ChangeUserRoleCommandHandler(
    ICallerResolver callers,
    IUserRepository users,
    IUnitOfWork unitOfWork) : IChangeUserRoleCommandHandler
{
    public async Task<Result<UserDto, ChangeUserRoleError>> HandleAsync(
        ChangeUserRoleCommand useCase,
        CancellationToken cancellationToken = default)
    {
        Caller caller = await callers.GetRequiredAsync(cancellationToken);
        if (useCase.UserId == caller.Id)
        {
            return new OwnRoleChange();
        }

        if (await users.GetByIdAsync(useCase.UserId, cancellationToken) is not { } user)
        {
            return new UserNotFound(useCase.UserId);
        }

        user.ChangeRole(useCase.Change.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return UserDto.From(user);
    }
}
