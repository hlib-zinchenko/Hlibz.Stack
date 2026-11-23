namespace Hlibz.Stack.Core.Repositories;

/// <summary>Saves what the repositories loaded and changed, or staged.</summary>
public interface IUnitOfWork
{
    /// <summary>Saves every change in one transaction.</summary>
    /// <param name="cancellationToken">Cancels the save.</param>
    /// <returns>A task that completes when the changes are saved.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
