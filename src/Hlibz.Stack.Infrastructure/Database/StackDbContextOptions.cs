namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>The provider and plugins every <see cref="StackDbContext"/> is built with.</summary>
internal static class StackDbContextOptions
{
    public static DbContextOptionsBuilder<StackDbContext> UseStackSqlite(
        this DbContextOptionsBuilder<StackDbContext> options,
        string connectionString) =>
        options.UseSqlite(connectionString).UseSnakeCaseNamingConvention();
}
