namespace Hlibz.Stack.Api.Endpoints.Users;

internal sealed class UsersEndpointGroup : IEndpointGroup
{
    public string Prefix => "api/users";

    public void Configure(RouteGroupBuilder group) =>
        group.WithTags("Users").RequireAuthorization();
}
