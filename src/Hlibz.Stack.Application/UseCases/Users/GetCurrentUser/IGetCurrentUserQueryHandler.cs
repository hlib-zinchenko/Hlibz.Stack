using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.GetCurrentUser;

/// <summary>Gets the signed-in user.</summary>
public interface IGetCurrentUserQueryHandler : IUseCaseHandler<GetCurrentUserQuery, UserDto, GetCurrentUserError>;
