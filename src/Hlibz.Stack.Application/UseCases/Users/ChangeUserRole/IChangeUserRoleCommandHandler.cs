using Hlibz.Stack.Application.Dto;

namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

/// <summary>Changes what a user may do.</summary>
public interface IChangeUserRoleCommandHandler : IUseCaseHandler<ChangeUserRoleCommand, UserDto, ChangeUserRoleError>;
