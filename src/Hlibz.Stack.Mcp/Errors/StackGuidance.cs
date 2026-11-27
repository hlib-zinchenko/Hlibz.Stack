using Hlibz.Results.Mcp;
using Hlibz.Stack.Application.UseCases.TodoLists;
using Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;
using Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;
using Hlibz.Stack.Application.UseCases.Users;
using Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;

namespace Hlibz.Stack.Mcp.Errors;

/// <summary>
/// The guidance for every error a tool can return: what the model should do next, with this
/// server's tool names. The error's message says what failed, for logs. Placeholders are the
/// error's details as MCP clients see them (camelCase). The host doesn't start while a tool's error
/// has none, or a template names a detail the error doesn't have.
/// </summary>
internal static class StackGuidance
{
    public static void Configure(ToolErrorOptions errors) => errors
        .Guide<TodoListNotFound>(
            "No todo list with id {todoListId} is visible to you: it doesn't exist, or it's another "
            + "user's. Call list_todo_lists to find the list's id.")
        .Guide<TodoItemNotFound>(
            "The list has no item with id {itemId}. Call get_todo_list to see its items and their ids.")
        .Guide<TodoListTitleTaken>(
            "The list's owner already has a todo list with this title (id {todoListId}). Use that list "
            + "(get_todo_list), or ask the user for another title.")
        .Guide<TodoListLimitReached>(
            "The user already has {maxTodoLists} todo lists, the most allowed. Ask the user which list to "
            + "delete (delete_todo_list), or add to one of their lists instead.")
        .Guide<TodoListFull>(
            "The list already holds {maxItems} items, the most it can. Ask the user whether to remove "
            + "items (remove_todo_item, done ones first), or to put this one on another list.")
        .Guide<TodoItemTitleTaken>(
            "The list already has this item (id {itemId}). Don't add it again; to change it, call "
            + "update_todo_item with that id.")
        .Guide<UserNotFound>(
            "No user has id {userId}. Call admin_list_users to find the user's id.")
        .Guide<OwnRoleChange>(
            "Admins can't change their own role. Tell the user another admin has to do it; retrying won't help.")
        .Guide<ValidationFailed>(
            "Fix these arguments and call the tool again: {errors}")
        .Guide<Forbidden>(
            "Only admins may do this. Tell the user; retrying won't help.")
        .Guide<Unauthorized>(
            "The access token no longer names a user. Ask the user to log in again (the Api's POST "
            + "api/auth/login) and update the token; retrying won't help.");
}
