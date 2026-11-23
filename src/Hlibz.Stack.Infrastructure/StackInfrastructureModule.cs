using System.Text;

using Hlibz.Stack.Application.Auth;
using Hlibz.Stack.Application.ReadRepositories;
using Hlibz.Stack.Infrastructure.Auth;
using Hlibz.Stack.Infrastructure.Database;
using Hlibz.Stack.Infrastructure.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hlibz.Stack.Infrastructure;

/// <summary>Registers the database, the repositories and the access token issuer.</summary>
public static class StackInfrastructureModule
{
    /// <summary>
    /// Registers the SQLite database (connection string <c>Stack</c>), the repositories the use
    /// cases load and read through, and <see cref="IAccessTokenIssuer"/> (section
    /// <c>Auth:Jwt</c>, checked at startup).
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The host's configuration.</param>
    /// <returns><paramref name="services"/>, for chaining.</returns>
    public static IServiceCollection AddStackInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Stack")
            ?? throw new InvalidOperationException("Set the connection string 'Stack'.");

        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<TimestampInterceptor>();
        services.AddDbContext<StackDbContext>((provider, options) =>
            ((DbContextOptionsBuilder<StackDbContext>)options)
                .UseStackSqlite(connectionString)
                .AddInterceptors(provider.GetRequiredService<TimestampInterceptor>()));
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<StackDbContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITodoListRepository, TodoListRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<ITodoListReadRepository, TodoListReadRepository>();

        services.AddOptions<SeedOptions>().BindConfiguration(SeedOptions.SectionName);
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .Validate(
                jwt => Encoding.UTF8.GetByteCount(jwt.SigningKey) >= 32,
                $"Set {JwtOptions.SectionName}:SigningKey to a secret of at least 32 bytes.")
            .ValidateOnStart();
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        return services;
    }
}
