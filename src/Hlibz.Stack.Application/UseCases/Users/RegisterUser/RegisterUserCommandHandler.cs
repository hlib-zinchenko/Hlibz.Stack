using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork) : IRegisterUserCommandHandler
{
    public async Task<Result<UserDto>> HandleAsync(
        RegisterUserCommand useCase,
        CancellationToken cancellationToken = default)
    {
        string email = User.NormalizeEmail(useCase.Email);
        if (await users.GetByEmailAsync(email, cancellationToken) is not null)
        {
            return Result<UserDto>.Conflict(StackErrors.EmailTaken);
        }

        User user = User.Create(email, useCase.Name, PasswordHasher.Hash(useCase.Password), Role.User);
        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<UserDto>.Success(UserDto.From(user));
    }
}
