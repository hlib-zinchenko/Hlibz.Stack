namespace Hlibz.Stack.Application.UseCases.Users.RegisterUser;

/// <summary>Someone already signed up with the email address.</summary>
[Error("stack_1001", ResultStatus.Conflict, "Someone already signed up with this email address.")]
public sealed record EmailTaken : Error;
