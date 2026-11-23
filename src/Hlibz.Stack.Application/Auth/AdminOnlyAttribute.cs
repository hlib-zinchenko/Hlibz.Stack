namespace Hlibz.Stack.Application.Auth;

/// <summary>A use case only admins may run.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AdminOnlyAttribute : Attribute;
