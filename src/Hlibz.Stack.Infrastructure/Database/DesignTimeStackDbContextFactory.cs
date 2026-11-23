using Microsoft.EntityFrameworkCore.Design;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>
/// Builds the context for <c>dotnet ef</c>, so migrations need no startup project:
/// <c>dotnet ef migrations add Name -p src/Hlibz.Stack.Infrastructure -o Database/Migrations</c>.
/// </summary>
internal sealed class DesignTimeStackDbContextFactory : IDesignTimeDbContextFactory<StackDbContext>
{
    public StackDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<StackDbContext>()
            .UseStackSqlite("Data Source=design-time.db")
            .Options);
}
