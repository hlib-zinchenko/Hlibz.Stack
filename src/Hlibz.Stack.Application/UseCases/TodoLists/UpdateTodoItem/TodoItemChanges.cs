namespace Hlibz.Stack.Application.UseCases.TodoLists.UpdateTodoItem;

/// <summary>What changes on a todo item: what's left out stays as it is.</summary>
/// <param name="Title">What's to be done.</param>
/// <param name="IsDone">Whether it's done.</param>
[RuleSummary("Set title, is_done, or both.")]
public sealed record TodoItemChanges(
    [MaxLength(TodoItem.TitleMaxLength)] string? Title = null,
    bool? IsDone = null) : IValidatableObject
{
    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Title is null && IsDone is null)
        {
            yield return new ValidationResult("Set title, is_done, or both.");
        }

        if (Title is not null && string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult("The title can't be blank.", [nameof(Title)]);
        }
    }
}
