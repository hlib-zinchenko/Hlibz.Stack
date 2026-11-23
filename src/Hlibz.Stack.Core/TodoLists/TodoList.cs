using Hlibz.Stack.Core.Entities;
using Hlibz.Stack.Core.Users;

namespace Hlibz.Stack.Core.TodoLists;

/// <summary>A user's todo list and the items on it.</summary>
public sealed class TodoList : Entity<TodoListId>
{
    /// <summary>The longest title.</summary>
    public const int TitleMaxLength = 100;

    private readonly List<TodoItem> _items = [];

    private TodoList(TodoListId id, UserId ownerId, string title)
        : base(id)
    {
        OwnerId = ownerId;
        Title = title;
    }

    /// <summary>Gets the user the list belongs to.</summary>
    public UserId OwnerId { get; private set; }

    /// <summary>Gets the list's title.</summary>
    public string Title { get; private set; }

    /// <summary>Gets the items, in their order on the list.</summary>
    public IReadOnlyList<TodoItem> Items => _items;

    /// <summary>Creates an empty list.</summary>
    /// <param name="ownerId">The user it belongs to.</param>
    /// <param name="title">Its title, trimmed here.</param>
    /// <returns>The list.</returns>
    public static TodoList Create(UserId ownerId, string title) =>
        new(EntityId.New<TodoListId>(), ownerId, title.Trim());

    /// <summary>Changes the list's title.</summary>
    /// <param name="title">The new title, trimmed here.</param>
    public void Rename(string title) => Title = title.Trim();

    /// <summary>Adds an item at the end of the list.</summary>
    /// <param name="title">What's to be done.</param>
    /// <returns>The new item.</returns>
    public TodoItem AddItem(string title)
    {
        int position = _items.Count == 0 ? 1 : _items.Max(item => item.Position) + 1;
        TodoItem item = TodoItem.Create(Id, title, position);
        _items.Add(item);
        return item;
    }

    /// <summary>Finds an item on the list.</summary>
    /// <param name="itemId">The item's id.</param>
    /// <returns>The item; <see langword="null"/> when it isn't on this list.</returns>
    public TodoItem? FindItem(TodoItemId itemId) => _items.Find(item => item.Id == itemId);

    /// <summary>Takes an item off the list.</summary>
    /// <param name="item">The item.</param>
    public void RemoveItem(TodoItem item) => _items.Remove(item);
}
