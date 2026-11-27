namespace Hlibz.Stack.Application.UseCases.Users.LogIn;

/// <summary>No user has that email address and password.</summary>
[Error("stack_1002", ResultStatus.Unauthorized, "The email address or password is wrong.")]
public sealed record InvalidCredentials : Error;
