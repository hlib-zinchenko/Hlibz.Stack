extern alias Mcp;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Hlibz.Stack.Tests;

/// <summary>
/// The Mcp host in Development, on the Api's database: the Api issues the tokens, and its users
/// are the ones the tools see.
/// </summary>
/// <param name="database">The Api's database file.</param>
public sealed class StackMcpFactory(string database) : WebApplicationFactory<Mcp::Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:Stack", $"Data Source={database}");
}
