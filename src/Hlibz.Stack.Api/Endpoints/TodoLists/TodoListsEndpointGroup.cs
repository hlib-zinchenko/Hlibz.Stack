namespace Hlibz.Stack.Api.Endpoints.TodoLists;

/// <summary>The signed-in user's todo lists; an admin reaches anyone's by its id.</summary>
internal sealed class TodoListsEndpointGroup : IEndpointGroup
{
    public string Prefix => "api/todo-lists";

    public void Configure(RouteGroupBuilder group) =>
        group.WithTags("Todo lists").RequireAuthorization();
}
