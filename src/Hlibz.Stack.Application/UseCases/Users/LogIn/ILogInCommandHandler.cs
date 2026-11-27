using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.LogIn;

/// <summary>Signs a user in.</summary>
public interface ILogInCommandHandler : IUseCaseHandler<LogInCommand, AccessTokenDto, LogInError>;
