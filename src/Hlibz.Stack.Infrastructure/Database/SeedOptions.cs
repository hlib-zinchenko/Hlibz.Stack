namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>The users an empty database starts with (section <c>Seed</c>).</summary>
public sealed class SeedOptions
{
    /// <summary>The configuration section.</summary>
    public const string SectionName = "Seed";

    /// <summary>Gets the users.</summary>
    public IList<SeedUser> Users { get; } = [];
}
