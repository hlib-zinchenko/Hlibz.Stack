using Hlibz.EntityFrameworkCore.ModelRules;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>The SQLite database: users, and their todo lists with the items on them.</summary>
/// <param name="options">The context's options.</param>
internal sealed class StackDbContext(DbContextOptions<StackDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    Task IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StackDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<UserId>().HaveConversion<EntityIdConverter<UserId>>();
        configurationBuilder.Properties<TodoListId>().HaveConversion<EntityIdConverter<TodoListId>>();
        configurationBuilder.Properties<TodoItemId>().HaveConversion<EntityIdConverter<TodoItemId>>();

        // SQLite can't order by a DateTimeOffset stored as text; as a number it can.
        configurationBuilder.Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToBinaryConverter>();

        configurationBuilder.ConfigureClientSetNullAs(DeleteBehavior.SetNull);
        configurationBuilder.UseModelRules(rules => rules
            .NoShadowProperties()
            .NamesFollow(NamingStyle.SnakeCase)
            .DecimalsHavePrecision()
            .StringsHaveMaxLength()
            .NullabilityMatchesClr()
            .EnumsStoredAsStrings()
            .NoClientSideDeleteBehaviors()
            .NoRedundantIndexes());
    }
}
