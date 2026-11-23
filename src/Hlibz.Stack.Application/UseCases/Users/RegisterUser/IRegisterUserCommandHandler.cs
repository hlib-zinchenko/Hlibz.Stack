using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.RegisterUser;

/// <summary>Signs a new user up.</summary>
public interface IRegisterUserCommandHandler : IUseCaseHandler<RegisterUserCommand, UserDto>;
