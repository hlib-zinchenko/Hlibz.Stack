using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hlibz.Stack.Infrastructure.Database.Configurations;

internal sealed class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.HasKey(list => list.Id);
        builder.Property(list => list.Title).HasMaxLength(TodoList.TitleMaxLength);

        // A user's lists go with them.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(list => list.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(list => list.Items)
            .WithOne()
            .HasForeignKey(item => item.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
