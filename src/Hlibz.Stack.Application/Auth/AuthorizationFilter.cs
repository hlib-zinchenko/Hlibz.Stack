namespace Hlibz.Stack.Application.Auth;

/// <summary>
/// Lets a use case through only for the callers it allows: everyone for
/// <see cref="AnonymousAttribute"/>, admins for <see cref="AdminOnlyAttribute"/>, any signed-in
/// user otherwise. It runs in every host, so HTTP and MCP get the same <c>401</c>/<c>403</c>.
/// </summary>
/// <param name="callers">Finds the caller.</param>
internal sealed class AuthorizationFilter(ICallerResolver callers) : IUseCaseFilter
{
    public async ValueTask<Result> InvokeAsync(UseCaseContext context, UseCaseFilterDelegate next)
    {
        if (context.UseCaseType.IsDefined(typeof(AnonymousAttribute), inherit: false))
        {
            return await next(context);
        }

        Caller? caller = await callers.GetAsync(context.CancellationToken);
        if (caller is null)
        {
            return Result.Unauthorized();
        }

        if (!caller.IsAdmin && context.UseCaseType.IsDefined(typeof(AdminOnlyAttribute), inherit: false))
        {
            return Result.Forbidden();
        }

        return await next(context);
    }
}
