using System.Reflection;

using Hlibz.Results.Mcp;
using Hlibz.Stack.Application.Auth;
using Hlibz.UseCases;

namespace Hlibz.Stack.Mcp.Errors;

/// <summary>
/// Which errors each tool can return, from the use case its handler runs (the tool takes the
/// handler's interface; the catalog names the handler class): the handler's error set,
/// <see cref="ValidationFailed"/> when the use case validates, <see cref="Forbidden"/> when it's
/// <see cref="AdminOnlyAttribute">admin only</see>, <see cref="ConcurrencyConflict"/> when the tool
/// isn't read-only (its save can find the rows changed meanwhile). So the tools declare none
/// themselves.
/// </summary>
/// <param name="useCases">The use cases.</param>
internal sealed class UseCaseToolErrors(IUseCaseCatalog useCases) : IToolErrorSource
{
    private readonly Dictionary<string, UseCaseDescriptor> _byTool = Map(useCases);

    /// <inheritdoc />
    public IEnumerable<Type> ErrorsOf(McpServerTool tool)
    {
        if (!_byTool.TryGetValue(tool.ProtocolTool.Name, out UseCaseDescriptor? useCase))
        {
            return [];
        }

        List<Type> errors = useCase.ErrorSetType is { } errorSet ? [.. ErrorSetDescriptor.For(errorSet).ErrorTypes] : [];
        if (useCase.CanFailValidation)
        {
            errors.Add(typeof(ValidationFailed));
        }

        if (useCase.UseCaseType.IsDefined(typeof(AdminOnlyAttribute), inherit: false))
        {
            errors.Add(typeof(Forbidden));
        }

        if (tool.ProtocolTool.Annotations?.ReadOnlyHint != true)
        {
            errors.Add(typeof(ConcurrencyConflict));
        }

        return errors;
    }

    // Each tool's use case: the first parameter that is a use case's handler interface.
    private static Dictionary<string, UseCaseDescriptor> Map(IUseCaseCatalog useCases)
    {
        Dictionary<string, UseCaseDescriptor> byTool = [];
        IEnumerable<MethodInfo> methods = typeof(UseCaseToolErrors).Assembly.GetTypes()
            .Where(type => type.IsDefined(typeof(McpServerToolTypeAttribute)))
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static));
        foreach (MethodInfo method in methods)
        {
            if (method.GetCustomAttribute<McpServerToolAttribute>()?.Name is { } name
                && method.GetParameters()
                    .Select(parameter => useCases.UseCases.FirstOrDefault(useCase =>
                        parameter.ParameterType.IsInterface && parameter.ParameterType.IsAssignableFrom(useCase.HandlerType)))
                    .FirstOrDefault(useCase => useCase is not null) is { } useCase)
            {
                byTool[name] = useCase;
            }
        }

        return byTool;
    }
}
