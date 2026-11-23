using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hlibz.Stack.Infrastructure.Database.Configurations;

internal sealed class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Title).HasMaxLength(TodoItem.TitleMaxLength);
        builder.HasIndex(item => new { item.TodoListId, item.Position });
    }
}
