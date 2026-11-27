extern alias Mcp;

using Hlibz.Stack.Infrastructure.Database;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The Mcp host in Development, on the Api's database: the Api issues the tokens, and its users
/// are the ones the tools see. <see cref="ConcurrentDelete"/> can delete a list under a save.
/// </summary>
/// <param name="database">The Api's database file.</param>
public sealed class StackMcpFactory(string database) : WebApplicationFactory<Mcp::Program>
{
    /// <summary>Gets the interceptor that deletes a list under a save.</summary>
    public ConcurrentDelete ConcurrentDelete => Services.GetRequiredService<ConcurrentDelete>();

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder
        .UseSetting("ConnectionStrings:Stack", $"Data Source={database}")
        .ConfigureTestServices(services => services
            .AddSingleton<ConcurrentDelete>()
            .ConfigureDbContext<StackDbContext>((provider, options) =>
                options.AddInterceptors(provider.GetRequiredService<ConcurrentDelete>())));
}
