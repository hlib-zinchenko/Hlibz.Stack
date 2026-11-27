using Hlibz.Stack.Application.Dto;
using Hlibz.Stack.Application.UseCases.TodoLists.AddTodoItem;
using Hlibz.Stack.Application.UseCases.TodoLists.CreateTodoList;
using Hlibz.Stack.Application.UseCases.TodoLists.DeleteTodoList;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoList;
using Hlibz.Stack.Application.UseCases.TodoLists.GetTodoLists;
using Hlibz.Stack.Application.UseCases.TodoLists.RemoveTodoItem;
using Hlibz.Stack.Application.UseCases.TodoLists.RenameTodoList;
using Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

namespace Hlibz.Stack.Mcp.Tools;

/// <summary>The signed-in user's todo lists; an admin reaches anyone's by its id.</summary>
[McpServerToolType]
internal static class TodoListTools
{
    [McpServerTool(Name = "list_todo_lists", Title = "List todo lists", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Lists your todo lists, newest first, with how many items each has and how many are done.")]
    public static async Task<PagedData<TodoListDto>> ListAsync(
        IGetTodoListsQueryHandler handler,
        [Description("Text the title contains.")] string? search = null,
        [Description("The page, from 1.")] int? page = null,
        [Description("Lists per page, up to 100.")] int? perPage = null,
        [Description("The order: title, item_count or created_at, each optionally :asc or :desc.")] string? sort = null,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new GetTodoListsQuery(null, search, page, perPage, sort),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "get_todo_list", Title = "Get a todo list", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Gets a todo list with its items, in order.")]
    public static async Task<TodoListDetailDto> GetAsync(
        IGetTodoListQueryHandler handler,
        [Description("The list's id.")] Guid todoListId,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new GetTodoListQuery(TodoListId.From(todoListId)),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "create_todo_list", Title = "Create a todo list", Destructive = false, OpenWorld = false)]
    [Description("Creates an empty todo list for you. Your lists' titles differ (ignoring case), and you may have up to 10 lists.")]
    public static async Task<TodoListDetailDto> CreateAsync(
        ICreateTodoListCommandHandler handler,
        [Description("The list's title, up to 100 characters.")] string title,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(new CreateTodoListCommand(title), cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "rename_todo_list", Title = "Rename a todo list", Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Renames a todo list, to a title none of the owner's other lists has.")]
    public static async Task<TodoListDetailDto> RenameAsync(
        IRenameTodoListCommandHandler handler,
        [Description("The list's id.")] Guid todoListId,
        [Description("The new title, up to 100 characters.")] string title,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new RenameTodoListCommand(TodoListId.From(todoListId), new TodoListChanges(title)),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "delete_todo_list", Title = "Delete a todo list", Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Deletes a todo list with all its items. It can't be undone.")]
    public static async Task<string> DeleteAsync(
        IDeleteTodoListCommandHandler handler,
        [Description("The list's id.")] Guid todoListId,
        CancellationToken cancellationToken = default)
    {
        (await handler.HandleAsync(
            new DeleteTodoListCommand(TodoListId.From(todoListId)),
            cancellationToken)).ThrowIfFailed();
        return "Deleted.";
    }

    [McpServerTool(Name = "add_todo_item", Title = "Add an item", Destructive = false, OpenWorld = false)]
    [Description("Adds an item at the end of a todo list. A list's items' titles differ (ignoring case), and a list holds up to 20 items.")]
    public static async Task<TodoItemDto> AddItemAsync(
        IAddTodoItemCommandHandler handler,
        [Description("The list's id.")] Guid todoListId,
        [Description("What's to be done, up to 200 characters.")] string title,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new AddTodoItemCommand(TodoListId.From(todoListId), new NewTodoItem(title)),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "update_todo_item", Title = "Change an item", Destructive = false, Idempotent = true, OpenWorld = false)]
    [Description("Renames an item, marks it done or to do again, or both. Leave out what stays as it is.")]
    public static async Task<TodoItemDto> UpdateItemAsync(
        IUpdateTodoItemCommandHandler handler,
        [Description("The list's id.")] Guid todoListId,
        [Description("The item's id.")] Guid itemId,
        [Description("The new title.")] string? title = null,
        [Description("Whether it's done.")] bool? isDone = null,
        CancellationToken cancellationToken = default) =>
        (await handler.HandleAsync(
            new UpdateTodoItemCommand(
                TodoListId.From(todoListId),
                TodoItemId.From(itemId),
                new TodoItemChanges(title, isDone)),
            cancellationToken)).ValueOrThrow();

    [McpServerTool(Name = "remove_todo_item", Title = "Remove an item", Destructive = true, Idempotent = false, OpenWorld = false)]
    [Description("Takes an item off a todo list.")]
    public static async Task<string> RemoveItemAsync(
        IRemoveTodoItemCommandHandler handler,
        [Description("The list's id.")] Guid todoListId,
        [Description("The item's id.")] Guid itemId,
        CancellationToken cancellationToken = default)
    {
        (await handler.HandleAsync(
            new RemoveTodoItemCommand(TodoListId.From(todoListId), TodoItemId.From(itemId)),
            cancellationToken)).ThrowIfFailed();
        return "Removed.";
    }
}
