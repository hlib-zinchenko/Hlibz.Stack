using Hlibz.Stack.Application.UseCases.TodoLists;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hlibz.Stack.Application;

/// <summary>Registers the use cases, whichever host runs them.</summary>
public static class StackApplicationModule
{
    /// <summary>
    /// Registers every use case handler wrapped in its pipeline: validation, the authorization
    /// filter (<see cref="AnonymousAttribute"/>, <see cref="AdminOnlyAttribute"/>), the handler.
    /// The host registers <see cref="ICurrentUser"/>; the infrastructure, the repositories and
    /// <see cref="IAccessTokenIssuer"/>.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns><paramref name="services"/>, for chaining.</returns>
    public static IServiceCollection AddStackApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<PasswordHasher>();
        services.AddScoped<ICallerResolver, CallerResolver>();
        services.AddScoped<TodoListAccess>();
        services.AddUseCases([typeof(StackApplicationModule).Assembly]);
        services.AddUseCaseFilter<AuthorizationFilter>();
        return services;
    }
}
