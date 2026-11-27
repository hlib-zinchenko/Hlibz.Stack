using Hlibz.Stack.Infrastructure.Database;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The Api in Development (its seeded admin and user, its signing key) on a database file of its
/// own, deleted afterwards; logging in is limited to 1000 calls a minute (the tests share one
/// client address), and <see cref="ConcurrentDelete"/> can delete a list under a save.
/// </summary>
public sealed class StackApiFactory : WebApplicationFactory<Program>
{
    private readonly string _database = Path.Combine(Path.GetTempPath(), $"stack-tests-{Guid.NewGuid():N}.db");

    /// <summary>Gets the database file, for the Mcp host to share.</summary>
    public string Database => _database;

    /// <summary>Gets the interceptor that deletes a list under a save.</summary>
    public ConcurrentDelete ConcurrentDelete => Services.GetRequiredService<ConcurrentDelete>();

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        SqliteConnection.ClearAllPools();
        File.Delete(_database);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder
        .UseSetting("ConnectionStrings:Stack", $"Data Source={_database}")
        .UseSetting("RateLimits:Auth:PermitLimit", "1000")
        .ConfigureTestServices(services => services
            .AddSingleton<ConcurrentDelete>()
            .ConfigureDbContext<StackDbContext>((provider, options) =>
                options.AddInterceptors(provider.GetRequiredService<ConcurrentDelete>())));
}
