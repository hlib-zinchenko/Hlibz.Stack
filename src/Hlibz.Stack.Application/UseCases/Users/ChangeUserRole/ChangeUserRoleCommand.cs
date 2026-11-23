namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

/// <summary>Changes what a user may do.</summary>
/// <param name="UserId">The user.</param>
/// <param name="Change">The new role.</param>
[AdminOnly]
public sealed record ChangeUserRoleCommand(UserId UserId, UserRoleChange Change) : IUseCase;
