namespace Hlibz.Stack.Api.Endpoints.Admin;

/// <summary>Users and their todo lists, for admins.</summary>
internal sealed class AdminUsersEndpointGroup : IAdminEndpointGroup
{
    public string Prefix => "api/admin/users";

    public void Configure(RouteGroupBuilder group) => group.WithTags("Admin: users");
}
