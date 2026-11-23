using Hlibz.Stack.Application.Auth;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>Brings the database up to date when a host starts.</summary>
public static class StackDatabase
{
    /// <summary>
    /// Applies the pending migrations, then, when there are no users yet, adds the configured
    /// <see cref="SeedOptions"/> users.
    /// </summary>
    /// <param name="services">The host's services.</param>
    /// <param name="cancellationToken">Cancels the update.</param>
    /// <returns>A task that completes when the database is up to date.</returns>
    public static async Task InitializeAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        StackDbContext db = scope.ServiceProvider.GetRequiredService<StackDbContext>();
        await db.Database.MigrateAsync(cancellationToken);

        SeedOptions seed = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;
        if (seed.Users.Count == 0 || await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        foreach (SeedUser user in seed.Users)
        {
            db.Users.Add(User.Create(user.Email, user.Name, PasswordHasher.Hash(user.Password), user.Role));
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
