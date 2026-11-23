using Hlibz.McpDocs.AspNetCore;
using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;
using Hlibz.Stack.Application.UseCases.Users.ChangeUserRole;
using Hlibz.Stack.Application.UseCases.Users.GetUsers;

namespace Hlibz.Stack.Mcp.Tools;

/// <summary>
/// Users and their lists, for admins. Every user sees these tools; the use cases turn away
/// non-admins. The todo list tools work on any user's list for an admin.
/// </summary>
[McpServerToolType]
[McpDocsGroup("Admin")]
internal static class AdminTools
{
    [McpServerTool(Name = "admin_list_users", Title = "List users", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Lists every user. Admins only.")]
    public static async Task<PagedData<UserDto>> ListUsersAsync(
        IGetUsersQueryHandler handler,
        [Description("Text the email address or name contains.")] string? search = null,
        [Description("The page, from 1.")] int? page = null,
        [Description("Users per page, up to 100.")] int? perPage = null,
        [Description("The order: email, name, role or created_at, each optionally :asc or :desc.")] string? sort = null,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new GetUsersQuery(search, page, perPage, sort),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "admin_list_user_todo_lists", Title = "List a user's todo lists", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Lists a user's todo lists. Admins only.")]
    public static async Task<PagedData<TodoListDto>> ListUserTodoListsAsync(
        IGetTodoListsQueryHandler handler,
        [Description("The user's id.")] Guid userId,
        [Description("Text the title contains.")] string? search = null,
        [Description("The page, from 1.")] int? page = null,
        [Description("Lists per page, up to 100.")] int? perPage = null,
        [Description("The order: title, item_count or created_at, each optionally :asc or :desc.")] string? sort = null,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new GetTodoListsQuery(UserId.From(userId), search, page, perPage, sort),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "admin_change_user_role", Title = "Change a user's role", Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Makes a user an admin, or a user again. Admins only, and not for themselves.")]
    public static async Task<UserDto> ChangeUserRoleAsync(
        IChangeUserRoleCommandHandler handler,
        [Description("The user's id.")] Guid userId,
        [Description("The new role.")] Role role,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new ChangeUserRoleCommand(UserId.From(userId), new UserRoleChange(role)),
            cancellationToken)).ValueOrThrow();
}
