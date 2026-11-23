namespace Hlibz.Stack.Application;

/// <summary>The error codes the use cases fail with.</summary>
public static class StackErrors
{
    /// <summary>Someone already signed up with the email address.</summary>
    public static readonly ErrorCode EmailTaken = new(
        "stack_1001",
        "Someone already signed up with this email address.");

    /// <summary>No user has that email address and password.</summary>
    public static readonly ErrorCode InvalidCredentials = new(
        "stack_1002",
        "The email address or password is wrong.");

    /// <summary>No todo list with that id is visible to the caller.</summary>
    public static readonly ErrorCode TodoListNotFound = new(
        "stack_1003",
        "There's no todo list with this id.");

    /// <summary>The todo list has no item with that id.</summary>
    public static readonly ErrorCode TodoItemNotFound = new(
        "stack_1004",
        "The todo list has no item with this id.");

    /// <summary>No user has that id.</summary>
    public static readonly ErrorCode UserNotFound = new(
        "stack_1005",
        "There's no user with this id.");

    /// <summary>An admin tried to change their own role.</summary>
    public static readonly ErrorCode OwnRoleChange = new(
        "stack_1006",
        "Admins can't change their own role, so there's always an admin left.");
}
