namespace Hlibz.Stack.Application.UseCases.Users;

/// <summary>No user has the id.</summary>
/// <param name="UserId">The user's id.</param>
[Error("stack_1005", ResultStatus.NotFound, "There's no user with this id.")]
public sealed record UserNotFound(UserId UserId) : Error;
