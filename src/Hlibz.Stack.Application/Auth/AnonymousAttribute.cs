namespace Hlibz.Stack.Application.Auth;

/// <summary>A use case anyone may run, signed in or not. Every other one needs a signed-in user.</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AnonymousAttribute : Attribute;
