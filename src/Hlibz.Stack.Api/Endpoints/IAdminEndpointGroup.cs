namespace Hlibz.Stack.Api.Endpoints;

/// <summary>
/// A group of admin endpoints: in the admin document, and signed-in only. The use cases
/// (<c>[AdminOnly]</c>) turn away everyone else with <c>403</c>.
/// </summary>
internal interface IAdminEndpointGroup : IEndpointGroup;
