using Hlibz.EntityFrameworkCore.ModelRules;
using Hlibz.Stack.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Hlibz.Stack.Tests;

public sealed class ModelRulesTests
{
    [Fact]
    public void Model_WithTheAppsRules_FollowsThem()
    {
        DbContextOptions<StackDbContext> options = new DbContextOptionsBuilder<StackDbContext>()
            .UseStackSqlite("Data Source=unused")
            .Options;

        ModelRuleVerifier.Verify(options);
    }
}
