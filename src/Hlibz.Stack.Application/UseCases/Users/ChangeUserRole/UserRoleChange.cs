namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

/// <summary>A user's new role.</summary>
/// <param name="Role">The role.</param>
public sealed record UserRoleChange([EnumDataType(typeof(Role))] Role Role);
