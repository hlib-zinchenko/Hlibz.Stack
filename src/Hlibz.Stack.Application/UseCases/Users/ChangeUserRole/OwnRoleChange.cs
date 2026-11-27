namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

/// <summary>An admin tried to change their own role.</summary>
[Error("stack_1006", ResultStatus.Error, "Admins can't change their own role, so there's always an admin left.")]
public sealed record OwnRoleChange : Error;
