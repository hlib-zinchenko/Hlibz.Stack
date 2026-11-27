using Hlibz.Results;
using Hlibz.UseCases;

namespace Hlibz.Stack.Infrastructure.Database;

/// <summary>
/// Turns a save that found its rows changed (EF Core's <see cref="DbUpdateConcurrencyException"/>:
/// an update or delete matched no row, because someone deleted it meanwhile) into the ambient
/// <see cref="ConcurrencyConflict"/>, which any use case's result accepts. EF Core stays here: the
/// handlers only call <see cref="IUnitOfWork.SaveChangesAsync"/>.
/// </summary>
internal sealed class ConcurrencyConflictFilter : IUseCaseFilter
{
    public async ValueTask<Result> InvokeAsync(UseCaseContext context, UseCaseFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ConcurrencyConflict();
        }
    }
}
