using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The Api in Development (its seeded admin and user, its signing key) on a database file of its
/// own, deleted afterwards.
/// </summary>
public sealed class StackApiFactory : WebApplicationFactory<Program>
{
    private readonly string _database = Path.Combine(Path.GetTempPath(), $"stack-tests-{Guid.NewGuid():N}.db");

    /// <summary>Gets the database file, for the Mcp host to share.</summary>
    public string Database => _database;

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        SqliteConnection.ClearAllPools();
        File.Delete(_database);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:Stack", $"Data Source={_database}");
}
