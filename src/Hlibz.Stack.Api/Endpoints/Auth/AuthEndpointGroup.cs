namespace Hlibz.Stack.Api.Endpoints.Auth;

internal sealed class AuthEndpointGroup : IEndpointGroup
{
    public string Prefix => "api/auth";

    public void Configure(RouteGroupBuilder group) => group.WithTags("Auth");
}
