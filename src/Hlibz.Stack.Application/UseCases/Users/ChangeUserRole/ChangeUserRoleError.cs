namespace Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

/// <summary>What changing a user's role fails with.</summary>
public union ChangeUserRoleError(UserNotFound, OwnRoleChange);
